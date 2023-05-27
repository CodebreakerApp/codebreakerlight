using CodeBreaker.Data.Models;

namespace CodeBreaker.APIs.Services;

// This interface represents the game service
public interface IGameService
{
    // Gets a game by GameId
    Task<Game?> GetAsync(Guid gameId);

    // Gets all the games by date
    IAsyncEnumerable<Game> GetByDateAsync(DateOnly date);

    // Creates a new game with the given username
    Task<Game> CreateAsync(string username);
}
