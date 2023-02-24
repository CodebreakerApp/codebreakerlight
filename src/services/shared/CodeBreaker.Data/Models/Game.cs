using static CodeBreaker.Shared.Colors;

namespace CodeBreaker.Data.Models;

public class Game {
    public required Guid GameId { get; init; }

    public required string Username { get; init; }

    public required IReadOnlyList<string> Code { get; init; }

    public required DateTime Start { get; init; }

    public DateTime? End { get; init; }

    public int Holes => Code.Count;

    public IReadOnlyList<string> Colors { get; init; } = new string[] { Black, White, Red, Blue, Yellow, Green };

    public IList<Move> Moves { get; init; } = new List<Move>();

    public int MaxMoves { get; init; } = 12;

    public bool Ended => End != null;

    public bool Won => Moves.LastOrDefault()?.KeyPegs?.Black == Holes;

    public override string ToString() =>
        $"{GameId} for {Username}, {string.Join("..", Code)}";
}
