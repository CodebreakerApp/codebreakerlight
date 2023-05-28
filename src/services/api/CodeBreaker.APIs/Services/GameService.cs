using CodeBreaker.APIs.Extensions;
using CodeBreaker.Data;
using CodeBreaker.Data.Models;
using CodeBreaker.Shared.Exceptions;

using static CodeBreaker.Shared.Colors;

namespace CodeBreaker.APIs.Services;

public class GameService : IGameService
{
    private readonly ICodeBreakerRepository _repository;

    private readonly ILogger _logger;

    public GameService(ICodeBreakerRepository repository, ILogger<GameService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Game> CreateAsync(string username)
    {
        Game game = CreateWithRandomCode(username);
        await _repository.CreateGameAsync(game);
        _logger.GameStarted(game.ToString());
        return game;
    }

    public async Task<Game?> GetAsync(Guid gameId) =>
        await _repository.GetGameAsync(gameId, false);

    public IAsyncEnumerable<Game> GetByDateAsync(DateOnly date) =>
        _repository.GetGamesByDateAsync(date);

    public virtual async Task<Game> CreateMoveAsync(Guid gameId, Move move)
    {
        Game game = await _repository.GetGameAsync(gameId) ?? throw new GameNotFoundException($"Game with id {gameId} not found");
        game.ApplyMove(move);
        await _repository.UpdateGameAsync(game);
        _logger.LogInformation("Set move");
        _logger.SetMove(move.ToString(), move.KeyPegs?.ToString() ?? string.Empty);
        return game;
    }

    private static Game CreateWithRandomCode(string username)
    {
        var holes = 4;
        var colors = new string[] { Black, White, Red, Blue, Green, Yellow };
        return new() {
            GameId = Guid.NewGuid(),
            Username = username,
            Code = CreateRandomCode(holes, colors),
            Start = DateTime.Now,
            Colors = colors,
            MaxMoves = 12
        };
    }

    private static IReadOnlyList<string> CreateRandomCode(int holes, ICollection<string> colors)
    {
        var pegs = new string[holes];

        for (int i = 0; i < holes; i++)
        {
            int index = Random.Shared.Next(colors.Count);
            pegs[i] = colors.ElementAt(index);
        }

        return pegs;
    }
}
