using CodeBreaker.Shared.Api;

namespace CodeBreaker.Services;

public interface IGameClient
{
    Task<CreateMoveResponse> SetMoveAsync(Guid gameId, params string[] colorNames);

    Task<CreateGameResponse> StartGameAsync(string username);
}
