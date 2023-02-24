namespace CodeBreaker.Shared.Api;

public record class MoveDto(
    int MoveNumber,
    IReadOnlyList<string> GuessPegs,
    KeyPegsDto? KeyPegs
);

public readonly record struct CreateMoveResponse(
    KeyPegsDto KeyPegs,
    bool Ended,
    bool Won
);

public readonly record struct CreateMoveRequest(IReadOnlyList<string> GuessPegs);

public readonly record struct KeyPegsDto(int Black, int White);