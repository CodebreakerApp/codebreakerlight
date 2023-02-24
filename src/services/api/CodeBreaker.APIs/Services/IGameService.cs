using CodeBreaker.Data.Models;

namespace CodeBreaker.APIs.Services;

public interface IGameService
{
    // Get
    Task<Game?> GetAsync(Guid gameId);
    
    // GetByDate
    IAsyncEnumerable<Game> GetByDateAsync(DateOnly date);

    // GetByDate
    IAsyncEnumerable<Game> GetByDateAsync(DateTime datetime);

    // Create
    Task<Game> CreateAsync(string username);
}
