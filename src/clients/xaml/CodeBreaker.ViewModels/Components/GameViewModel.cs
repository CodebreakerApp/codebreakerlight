using System.Collections.ObjectModel;
using CodeBreaker.Shared.Api;

namespace CodeBreaker.ViewModels.Components;

public class GameViewModel
{
    private readonly GameDto _game;

    public GameViewModel(GameDto game)
    {
        _game = game;
    }

    public Guid GameId => _game.GameId;

    public string Name => _game.Username;

    public IReadOnlyList<string> Code => _game.Code;

    public IReadOnlyList<string> ColorList => _game.Colors;

    public int Holes => _game.Holes;

    public int MaxMoves => _game.MaxMoves;

    public DateTime StartTime => _game.Start;

    public ObservableCollection<MoveViewModel> Moves { get; init; } = new();
}
