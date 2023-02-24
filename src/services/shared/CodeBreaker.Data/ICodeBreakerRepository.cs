using CodeBreaker.Data.Models;

namespace CodeBreaker.Data;

public interface ICodeBreakerRepository
{
    Task CreateGameAsync(Game game);

    Task<Game?> GetGameAsync(Guid gameId, bool withTracking = true);

    IAsyncEnumerable<Game> GetGamesByDateAsync(DateOnly date);

    public IAsyncEnumerable<Game> GetGamesByDateAsync(DateTime datetime) =>
        GetGamesByDateAsync(DateOnly.FromDateTime(datetime));

    Task UpdateGameAsync(Game game);
}
