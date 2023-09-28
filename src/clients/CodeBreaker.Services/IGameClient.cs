using CodeBreaker.Shared.Models.Api;

namespace CodeBreaker.Services;

public interface IGameClient
{
    Task<CreateMoveResponse> SetMoveAsync(Guid gameId, params string[] colorNames);

    Task<CreateGameResponse> StartGameAsync(string username, string gameType);

    Task CancelGameAsync(Guid gameId);
}
