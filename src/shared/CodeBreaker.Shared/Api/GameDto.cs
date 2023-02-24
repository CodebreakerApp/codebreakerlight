namespace CodeBreaker.Shared.Api;

public record class GameDto(
    Guid GameId,
    IReadOnlyList<string> Code,
    int Holes,
    IReadOnlyList<string> Colors,
    int MaxMoves,
    string Username,
    DateTime Start,
    DateTime? End,
    IEnumerable<MoveDto> Moves
);

public readonly record struct GetGamesResponse(IEnumerable<GameDto> Games);

public readonly record struct GetGameResponse(GameDto Game);

public readonly record struct CreateGameResponse(GameDto Game);

public readonly record struct CreateGameRequest(string Username);