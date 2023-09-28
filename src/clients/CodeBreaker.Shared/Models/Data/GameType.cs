using static CodeBreaker.Shared.Models.Data.Colors;

namespace CodeBreaker.Shared.Models.Data;

public class GameType : GameType<string>
{
    public GameType(string name, IReadOnlyList<string> fields, int holes, int maxMoves) : base(name, fields, holes, maxMoves)
    {
    }

    public static GameType Default =>
        new (
        "6x4Game",
            new string[] { Black, White, Red, Green, Blue, Yellow },
            4,
            12
        );
}

public class GameType<TField>
{
    private readonly string _name = string.Empty;

    private readonly int _holes = 0;

    private readonly int _maxMoves = 0;

    public GameType(string name, IReadOnlyList<TField> fields, int holes, int maxMoves)
    {
        Name = name;
        Fields = fields;
        Holes = holes;
        MaxMoves = maxMoves;
    }

    public string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(Name));

            _name = value;
        }
    }

    public IReadOnlyList<TField> Fields { get; init; } = new List<TField>();

    public int Holes
    {
        get => _holes;
        init
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Holes));

            _holes = value;
        }
    }

    public int MaxMoves
    {
        get => _maxMoves;
        init
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(MaxMoves));

            _maxMoves = value;
        }
    }

    public override string ToString() => Name;
}
