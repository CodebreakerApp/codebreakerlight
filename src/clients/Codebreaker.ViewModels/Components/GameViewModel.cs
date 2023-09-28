namespace Codebreaker.ViewModels.Components;

public class GameViewModel
{
    private readonly Game _game;

    public GameViewModel(Game game)
    {
        _game = game;
    }

    public Guid GameId => _game.GameId;

    public string Name => _game.PlayerName;

    public GameType GameType => _game.GameType;

    public IDictionary<string, string[]> FieldValues => _game.FieldValues;

    public int NumberCodes => _game.NumberCodes;

    public int MaxMoves => _game.MaxMoves;

    public DateTime StartTime => _game.StartTime;

    public ObservableCollection<MoveViewModel> Moves { get; } = new ObservableCollection<MoveViewModel>();
}
