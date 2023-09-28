namespace CodeBreaker.Shared.Models.Data;

public class Game
{
    private readonly Guid _id;

    private readonly string _name = string.Empty;

    private readonly DateTime _start = DateTime.Now;

    private DateTime? _end;

    public Game()
    {
    }

    public Game(Guid gameId, GameType<string> type, string username, IReadOnlyList<string> code)
    {
        GameId = gameId;
        Type = type;
        Username = username;
        Code = new List<string>(code);
    }

    public Game(Guid gameId, GameType<string> type, string username, IReadOnlyList<string> code, DateTime start, DateTime? end, IList<Move> moves)
        : this(gameId, type, username, code)
    {
        Start = start;
        End = end;
        Moves = moves;
    }

    /// <summary>
    /// The Id of the game.
    /// Used as partitionKey and primaryKey in Cosmos.
    /// </summary>
    public Guid GameId
    {
        get => _id;
        init
        {
            if (value == default)
                throw new ArgumentException(nameof(GameId));

            _id = value;
        }
    }

    public GameType<string> Type { get; init; } = GameType.Default;

    public string Username
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(Username));

            _name = value;
        }
    }

    public IReadOnlyList<string> Code { get; init; } = new List<string>();

    public DateTime Start
    {
        get => _start;
        init
        {
            if (value == default)
                throw new ArgumentException(nameof(Start));

            _start = value;
        }
    }

    public DateTime? End
    {
        get => _end;
        set
        {
            if (value < Start)
                throw new ArgumentOutOfRangeException(nameof(End));

            _end = value;
        }
    }

    public bool Ended => End != null;

    public bool Won => Moves.LastOrDefault()?.KeyPegs?.Black == Type.Holes;

    public IList<Move> Moves { get; init; } = new List<Move>();

    public override string ToString() =>
        $"{GameId} for {Username}, {string.Join("..", Code)}";
}
