using System.Runtime.Serialization;

namespace CodeBreaker.Shared.Models.Data;

public class Move : Move<string>
{
    public Move()
    {
    }

    public Move(IReadOnlyList<string> guessPegs)
    {
        GuessPegs = guessPegs;
    }

    public Move(int moveNumber, IReadOnlyList<string> guessPegs)
        : this(guessPegs)
    {
        MoveNumber = moveNumber;
    }

    public Move(int moveNumber, IReadOnlyList<string> guessPegs, KeyPegs? keyPegs)
        : this(moveNumber, guessPegs)
    {
        KeyPegs = keyPegs;
    }
}

public class Move<TField>
{
    private int _moveNumber;

    public Move()
    {
    }

    public Move(IReadOnlyList<TField> guessPegs)
    {
        GuessPegs = guessPegs;
    }

    public Move(int moveNumber, IReadOnlyList<TField> guessPegs)
        : this(guessPegs)
    {
        MoveNumber = moveNumber;
    }

    public Move(int moveNumber, IReadOnlyList<TField> guessPegs, KeyPegs? keyPegs)
        : this(moveNumber, guessPegs)
    {
        KeyPegs = keyPegs;
    }

    public int MoveNumber
    {
        get => _moveNumber;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(MoveNumber));

            _moveNumber = value;
        }
    }

    public IReadOnlyList<TField> GuessPegs { get; init; } = new List<TField>();

    public KeyPegs? KeyPegs { get; set; }

    public override string ToString() =>
        $"{MoveNumber}, {string.Join("..", GuessPegs)}";
}

public record struct KeyPegs
{
    private int _black = 0;

    private int _white = 0;

    public KeyPegs()
    {
    }

    public KeyPegs(int black, int white)
    {
        Black = black;
        White = white;
    }

    public int Black
    {
        get => _black;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Black));

            _black = value;
        }
    }

    public int White
    {
        get => _white;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(White));

            _white = value;
        }
    }

    [IgnoreDataMember]
    public int Total => Black + White;
}
