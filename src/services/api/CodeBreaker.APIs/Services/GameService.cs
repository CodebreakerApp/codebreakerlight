using CodeBreaker.Data;
using CodeBreaker.Data.Models;
using static CodeBreaker.Shared.Colors;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        _logger.LogInformation("Game started");
        return game;
    }

    public async Task<Game?> GetAsync(Guid gameId) =>
        await _repository.GetGameAsync(gameId, false);

    public IAsyncEnumerable<Game> GetByDateAsync(DateOnly date) =>
        _repository.GetGamesByDateAsync(date);

    public IAsyncEnumerable<Game> GetByDateAsync(DateTime datetime) =>
        GetByDateAsync(DateOnly.FromDateTime(datetime));

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
