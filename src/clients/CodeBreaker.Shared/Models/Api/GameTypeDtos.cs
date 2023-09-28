namespace CodeBreaker.Shared.Models.Api;

public readonly record struct GameTypeDto(string Name);

public readonly record struct GetGameTypesResponse(
    IEnumerable<GameTypeDto> GameTypes
);
