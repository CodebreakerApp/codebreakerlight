using CodeBreaker.Services;
using CodeBreaker.Shared.Api;
using CodeBreaker.ViewModels.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace CodeBreaker.ViewModels;

public enum GameMode
{
    NotRunning,
    Started,
    MoveSet,
    Lost,
    Won
}

public enum GameMoveValue
{
    Started,
    Completed
}

public class GamePageViewModelOptions
{
    public bool EnableDialogs { get; set; } = false;
}

public partial class GamePageViewModel : ObservableObject
{
    private readonly IGameClient _client;
    private int _moveNumber = 0;
    private GameDto? _game;
    private readonly bool _enableDialogs = false;
    private readonly IDialogService _dialogService;

    public GamePageViewModel(
        IGameClient client,
        IOptions<GamePageViewModelOptions> options,
        IDialogService dialogService)
    {
        _client = client;
        _dialogService = dialogService;
        _enableDialogs = options.Value.EnableDialogs;

        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(GameStatus))
                WeakReferenceMessenger.Default.Send(new GameStateChangedMessage(GameStatus));
        };
    }

    public InfoBarMessageService InfoBarMessageService { get; } = new();

    public GameDto? Game
    {
        get => _game;
        set
        {
            OnPropertyChanging(nameof(Game));
            OnPropertyChanging(nameof(Fields));
            _game = value;

            Fields.Clear();

            for (int i = 0; i < value?.Holes; i++)
            {
                SelectedFieldViewModel field = new();
                field.PropertyChanged += (sender, e) => SetMoveCommand.NotifyCanExecuteChanged();
                Fields.Add(field);
            }

            OnPropertyChanged(nameof(Game));
            OnPropertyChanged(nameof(Fields));
        }
    }

    [ObservableProperty]
    private string _name = string.Empty;

    [NotifyPropertyChangedFor(nameof(IsNameEnterable))]
    [ObservableProperty]
    private bool _isNamePredefined = false;

    public ObservableCollection<SelectedFieldViewModel> Fields { get; } = new();

    public ObservableCollection<SelectionAndKeyPegs> GameMoves { get; } = new();

    [ObservableProperty]
    private GameMode _gameStatus = GameMode.NotRunning;

    [NotifyPropertyChangedFor(nameof(IsNameEnterable))]
    [ObservableProperty]
    private bool _inProgress = false;

    [ObservableProperty]
    private bool _isCancelling = false;

    public bool IsNameEnterable => !InProgress && !IsNamePredefined;

    [RelayCommand(AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
    private async Task StartGameAsync()
    {
        try
        {
            InitializeValues();

            InProgress = true;
            CreateGameResponse response = await _client.StartGameAsync(Name);

            GameStatus = GameMode.Started;

            Game = response.Game;
            _moveNumber++;
        }
        catch (Exception ex)
        {
            InfoMessageViewModel message = InfoMessageViewModel.Error(ex.Message);
            message.ActionCommand = new RelayCommand(() =>
            {
                GameStatus = GameMode.NotRunning;
                message.Close();
            });
            InfoBarMessageService.ShowMessage(message);

            if (_enableDialogs)
                await _dialogService.ShowMessageAsync(ex.Message);
        }
        finally
        {
            InProgress = false;
        }
    }


    [RelayCommand(CanExecute = nameof(CanSetMove), AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
    private async Task SetMoveAsync()
    {
        try
        {
            InProgress = true;
            WeakReferenceMessenger.Default.Send(new GameMoveMessage(GameMoveValue.Started));

            if (Game is null)
                throw new InvalidOperationException("No game running");

            if (Fields.Count != Game.Holes || Fields.Any(x => !x.IsSet))
                throw new InvalidOperationException("All colors need to be selected before invoking this method");

            string[] selection = Fields.Select(x => x.Value!).ToArray();

            CreateMoveResponse response = await _client.SetMoveAsync(Game.GameId, selection);

            SelectionAndKeyPegs selectionAndKeyPegs = new(selection, response.KeyPegs, _moveNumber++);
            GameMoves.Add(selectionAndKeyPegs);
            GameStatus = GameMode.MoveSet;

            WeakReferenceMessenger.Default.Send(new GameMoveMessage(GameMoveValue.Completed, selectionAndKeyPegs));

            if (response.Won)
            {
                GameStatus = GameMode.Won;
                InfoBarMessageService.ShowInformation("Congratulations - you won!");

                if (_enableDialogs)
                    await _dialogService.ShowMessageAsync("Congratulations - you won!");
            }
            else if (response.Ended)
            {
                GameStatus = GameMode.Lost;
                InfoBarMessageService.ShowInformation("Sorry, you didn't find the matching colors!");

                if (_enableDialogs)
                    await _dialogService.ShowMessageAsync("Sorry, you didn't find the matching colors!");
            }
        }
        catch (Exception ex)
        {
            InfoBarMessageService.ShowError(ex.Message);

            if (_enableDialogs)
                await _dialogService.ShowMessageAsync(ex.Message);
        }
        finally
        {
            ClearSelectedColor();
            InProgress = false;
        }
    }

    private bool CanSetMove =>
        Fields.All(s => s is not null && s.IsSet);

    private void ClearSelectedColor()
    {
        for (int i = 0; i < Fields.Count; i++)
            Fields[i].Reset();

        SetMoveCommand.NotifyCanExecuteChanged();
    }

    private void InitializeValues()
    {
        ClearSelectedColor();
        GameMoves.Clear();
        GameStatus = GameMode.NotRunning;
        InfoBarMessageService.Clear();
        _moveNumber = 0;
    }
}

public record SelectionAndKeyPegs(string[] GuessPegs, KeyPegsDto KeyPegs, int MoveNumber);

public record class GameStateChangedMessage(GameMode GameMode);

public record class GameMoveMessage(GameMoveValue GameMoveValue, SelectionAndKeyPegs? SelectionAndKeyPegs = null);
