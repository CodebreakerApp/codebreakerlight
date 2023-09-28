using CodeBreaker.Shared.Models.Data;

namespace CodeBreaker.Shared.Models.Api;

public readonly record struct GameDto(
    Guid GameId,
    IReadOnlyList<string> Code,
    GameType<string> Type,
    string Username,
    DateTime Start,
    DateTime? End,
    IEnumerable<MoveDto<string>> Moves
);

public readonly record struct GetGamesResponse(IEnumerable<GameDto> Games);

public readonly record struct GetGameResponse(GameDto Game);

public readonly record struct CreateGameResponse(GameDto Game);

public readonly record struct CreateGameRequest(
    string Username,
    string GameType
);
