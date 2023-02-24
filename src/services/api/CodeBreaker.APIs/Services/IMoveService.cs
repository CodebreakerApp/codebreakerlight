using CodeBreaker.Data.Models;

namespace CodeBreaker.APIs.Services;

public interface IMoveService
{
    Task<Game> CreateMoveAsync(Guid gameId, Move move);
}
