using CodeBreaker.Data.Models;
using CodeBreaker.Data;
using CodeBreaker.APIs.Extensions;
using CodeBreaker.Shared.Exceptions;

namespace CodeBreaker.APIs.Services;

public class MoveService : IMoveService {
    private readonly ICodeBreakerRepository _repository;

    private readonly ILogger _logger;

    public MoveService(ICodeBreakerRepository repository, ILogger<GameService> logger) {
        _repository = repository;
        _logger = logger;
    }

    public virtual async Task<Game> CreateMoveAsync(Guid gameId, Move move) {
        Game game = await _repository.GetGameAsync(gameId) ?? throw new GameNotFoundException($"Game with id {gameId} not found");
        game.ApplyMove(move);
        await _repository.UpdateGameAsync(game);
        _logger.LogInformation("Set move");
        return game;
    }
}