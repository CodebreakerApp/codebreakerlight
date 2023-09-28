namespace CodeBreaker.Shared.Models.Api;

public readonly record struct MoveDto<TField>(
    int MoveNumber,
    IReadOnlyList<TField> GuessPegs,
    KeyPegsDto? KeyPegs
);

public readonly record struct CreateMoveResponse(
    KeyPegsDto KeyPegs,
    bool Ended,
    bool Won
);

public readonly record struct CreateMoveRequest(IReadOnlyList<string> GuessPegs);

public readonly record struct KeyPegsDto(
    int Black,
    int White
);
