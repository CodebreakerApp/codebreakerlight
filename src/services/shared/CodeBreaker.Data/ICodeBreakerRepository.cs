using CodeBreaker.Data.Models;

namespace CodeBreaker.Data;

public interface ICodeBreakerRepository
{
    Task CreateGameAsync(Game game);

    Task<Game?> GetGameAsync(Guid gameId, bool withTracking = true);

    IAsyncEnumerable<Game> GetGamesByDateAsync(DateOnly date);

    Task UpdateGameAsync(Game game);
}
