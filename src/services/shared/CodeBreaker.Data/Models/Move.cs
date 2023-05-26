namespace CodeBreaker.Data.Models;

public class Move 
{
    public required int MoveNumber { get; set; }

    public required IReadOnlyList<string> GuessPegs { get; init; }

    public KeyPegs? KeyPegs { get; set; }

    public override string ToString() =>
        $"{MoveNumber}, {string.Join("..", GuessPegs)}";
}

public readonly record struct KeyPegs(int Black, int White)
{
    public readonly int Total => Black + White;
}