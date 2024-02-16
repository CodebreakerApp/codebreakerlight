# Part 1 - Minimal API

> The game will be accessible via a REST API implemented with ASP.NET Core.

## A Model

> This is information abou the model used, the model is already part of this NuGet package: [CNinnovation.Codebreaker.BackendModels](https://www.nuget.org/packages/CNinnovation.Codebreaker.BackendModels/).

Check the code of the *Codebreaker.GameAPIs.Models* project. The classes `Game` and `Move` represent a stored game.

```csharp
public class Game(
    Guid gameId,
    string gameType,
    string playerName,
    DateTime startTime,
    int numberCodes,
    int maxMoves) : IGame
{
    public Guid GameId { get; } = gameId;
    public string GameType { get; } = gameType;
    public string PlayerName { get; } = playerName;
    public DateTime StartTime { get; } = startTime;
    public DateTime? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public int LastMoveNumber { get; set; } = 0;
    public int NumberCodes { get; private set; } = numberCodes;
    public int MaxMoves { get; private set; } = maxMoves;
    public bool IsVictory { get; set; } = false;

    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }

    public required string[] Codes { get; init; }
    public ICollection<Move> Moves { get; } = new List<Move>();

    public override string ToString() => $"{GameId}:{GameType} - {StartTime}";
}
```

```csharp
public class Move(Guid moveId, int moveNumber)
{
    public Guid MoveId { get; private set; } = moveId;
    public int MoveNumber { get; } = moveNumber;
    public required string[] GuessPegs { get; init; }
    public required string[] KeyPegs { get; init; }

    public override string ToString() => $"{MoveNumber}. {string.Join(':', GuessPegs)}";
}
```

The `Game` class implements the `IGame` interface which is used by game analyers zu analyze a game move.

## The Minimal API

Create an ASP.NET Core 8 project hosting the *Minimal API*

### In-Memory Games Repository

Create an in-memory games repository class `GamesMemoryRepository` implementing the interface `IGamesRepository`. The most important methods are `AddGameAsync`, `AddMoveAsync`, and `GetGameAsync`. All the other methods can throw a `NotSupportedException` with a simple game flow.

```csharp
public partial class GamesMemoryRepository(ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();
    private DateTime _lastCleanupRun = DateTime.MinValue;
    private bool _cleanupRunnerActive = false;
    private static readonly object _cleanupLock = new();

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        if (!_games.TryAdd(game.Id, game))
        {
            Log.GameExists(logger, game.Id);
            
        }
        _ = CleanupOldGamesAsync(); // don't need to wait for this to complete

        return Task.CompletedTask;
    }

    public Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_games.TryRemove(id, out _))
        {
            Log.GameNotFound(logger, id);
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    public Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(id, out Game? game);
        return Task.FromResult(game);
    }

    public Task AddMoveAsync(Game game, Move _, CancellationToken cancellationToken = default)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> filteredGames = _games.Values;

        if (!string.IsNullOrEmpty(gamesQuery.PlayerName))
        {
            filteredGames = filteredGames.Where(g => g.PlayerName.Equals(gamesQuery.PlayerName));
        }

        if (gamesQuery.Date != null)
        {
            filteredGames = filteredGames.Where(g => DateOnly.FromDateTime(g.StartTime) == gamesQuery.Date);
        }

        if (gamesQuery.RunningOnly)
        {
            filteredGames = filteredGames.Where(g => !g.Ended());
        }

        if (gamesQuery.Ended)
        {
            filteredGames = filteredGames.Where(g => g.Ended());
        }

        filteredGames = filteredGames.OrderBy(g => g.Duration).ThenBy(g => g.StartTime).Take(500);

        filteredGames = filteredGames.ToList();

        _ = CleanupOldGamesAsync(); // don't need to wait for this to complete

        return Task.FromResult(filteredGames);
    }

    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    private Task CleanupOldGamesAsync()
    {
        if (_lastCleanupRun > DateTime.Now.AddHours(-1))
        {
            return Task.CompletedTask;
        }
        lock (_cleanupLock)
        {
            if (!_cleanupRunnerActive)
            {
                return Task.CompletedTask;
            }
            _cleanupRunnerActive = true;
        }
        return Task.Run(() =>
        {
            _lastCleanupRun = DateTime.Now;

            logger.StartCleanupGames();
            var currentTime = DateTime.Now;
            var gamesToRemove = _games.Values.Where(g => g.StartTime <= currentTime.AddHours(-3)).ToList();
            int gamesRemoved = 0;
            foreach (var game in gamesToRemove)
            {
                if (_games.TryRemove(game.Id, out _))
                {
                    gamesRemoved++;
                }
            }
            logger.CleanedUpGames(gamesRemoved);
            lock (_cleanupLock)
            {
                _cleanupRunnerActive = false;
            }
        });
    }
}
```

### Games Factory

Create a `GamesFactory` class. The class from the repo supports different game types. Today we only need the Game6x4 game type, you can skip the other types. This factory uses a game analyzer from the analyzer library to calculate the move result.

```csharp
public static class GamesFactory
{
    private static readonly string[] s_colors6 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange];
    private static readonly string[] s_colors8 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown];
    private static readonly string[] s_colors5 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple];
    private static readonly string[] s_shapes5 = [Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Star, Shapes.Rectangle];

    /// <summary>
    /// Creates a game object with specified gameType and playerName.
    /// </summary>
    /// <param name="gameType">The type of game to be created.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>The created game object.</returns>
    public static Game CreateGame(string gameType, string playerName)
    {
        Game Create6x4SimpleGame() =>
            new(Guid.NewGuid(), gameType, playerName,  DateTime.UtcNow, 4, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors6 }
                },
                Codes = Random.Shared.GetItems(s_colors6, 4)
            };

        Game Create6x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 4, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors6 }
                },
                Codes = Random.Shared.GetItems(s_colors6, 4)
            };

        Game Create8x5Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 5, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors8 }
                },
                Codes = Random.Shared.GetItems(s_colors8, 5)
            };

        Game Create5x5x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 4, 14)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors5 },
                    { FieldCategories.Shapes, s_shapes5 }
                },
                Codes = Random.Shared.GetItems(s_shapes5, 4)
                    .Zip(Random.Shared.GetItems(s_colors5, 4), (shape, color) => (Shape: shape, Color: color))
                    .Select(item => string.Join(';', item.Shape, item.Color))
                    .ToArray()
            };
        
        return gameType switch
        {
            GameTypes.Game6x4Mini => Create6x4SimpleGame(),
            GameTypes.Game6x4 => Create6x4Game(),
            GameTypes.Game8x5 => Create8x5Game(),
            GameTypes.Game5x5x4 => Create5x5x4Game(),
            _ => throw new CodebreakerException("Invalid game type") { Code = CodebreakerExceptionCodes.InvalidGameType }
        };
    }

    /// <summary>
    /// Applies a player's move to a game and returns a <see cref="Move"/> object that encapsulates the player's guess and the result of the guess.
    /// Returns the Move and updates the Game
    /// </summary>
    /// <param name="game">The game to apply the move to.</param>
    /// <param name="guesses">The player's guesses.</param>
    /// <param name="moveNumber">The move number.</param>
    /// <returns>A <see cref="Move"/> object that encapsulates the player's guess and the result of the guess.</returns>
    public static Move ApplyMove(this Game game, string[] guesses, int moveNumber)
    {
        static TField[] GetGuesses<TField>(IEnumerable<string> guesses)
            where TField : IParsable<TField> => guesses
                .Select(g => TField.Parse(g, default))
                .ToArray();

        string[] GetColorGameGuessAnalyzerResult()
        {
            ColorGameGuessAnalyzer analyzer = new(game, GetGuesses<ColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] GetSimpleGameGuessAnalyzerResult()
        {
            SimpleGameGuessAnalyzer analyzer = new(game, GetGuesses<ColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] GetShapeGameGuessAnalyzerResult()
        {
            ShapeGameGuessAnalyzer analyzer = new(game, GetGuesses<ShapeAndColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] results = game.GameType switch
        {
            GameTypes.Game6x4 => GetColorGameGuessAnalyzerResult(),
            GameTypes.Game8x5 => GetColorGameGuessAnalyzerResult(),
            GameTypes.Game6x4Mini => GetSimpleGameGuessAnalyzerResult(),
            GameTypes.Game5x5x4 => GetShapeGameGuessAnalyzerResult(),
            _ => throw new CodebreakerException("Invalid game type") { Code = CodebreakerExceptionCodes.InvalidGameType }
        };

        Move move = new(Guid.NewGuid(), moveNumber)
        {
            GuessPegs = guesses,
            KeyPegs = results
        };

        game.Moves.Add(move);
        return move;
    }
}
```

### Games service contract and implementation

Define the `IGamesService` contract for a game run:

```csharp
public interface IGamesService
{
    /// <summary>
    /// Start a new game
    /// </summary>
    /// <param name="gameType">type of the game</param>
    /// <param name="playerName">name of the player</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>new game</returns>
    Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// set new moves
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="gameType">the type of the game</param>
    /// <param name="guesses">an enumerable guesses of strings</param>
    /// <param name="moveNumber">the number of the move</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>tuple consisting of the updated game and its result</returns>
    Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the Game by id
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>the game with the given id or null if the game was not found</returns>
    ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a game with the given id
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends and returns the game with the result
    /// </summary>
    /// <param name="id">the game id to end</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>ended game</returns>
    Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of games as an IEnumerable of Game
    /// </summary>
    /// <param name="gamesQuery">optional games query</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>IEnumerable of Game</returns>
    Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default);
}
```

## Models for the API data transfer

Implement the DTOs for the API data transfer (Models/GameAPIModels.cs):

```csharp
[JsonConverter(typeof(JsonStringEnumConverter<GameType>))]
public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(GameType GameType, string PlayerName);

public record class CreateGameResponse(Guid Id, GameType GameType, string PlayerName, int NumberCodes, int MaxMoves)
{
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }
}

public record class UpdateGameRequest(Guid Id, GameType GameType, string PlayerName, int MoveNumber, bool End = false)
{
    public string[]? GuessPegs { get; set; }
}

public record class UpdateGameResponse(
    Guid Id,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[]? Results);
```

### Game Endpoint

Define a game endpoint and define a group (Endpoints/GameEndpoints.cs):

```csharp
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games")
            .WithTags("Games API");
```

Using the group, specify MapPost to start a game:

```csharp
        group.MapPost("/", async Task<Results<Created<CreateGameResponse>, BadRequest<GameError>>> (
            CreateGameRequest request,
            IGamesService gameService,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            Game game;
            try
            {
                game = await gameService.StartGameAsync(request.GameType.ToString(), request.PlayerName, cancellationToken);
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.InvalidGameType)
            {
                GameError error = new(ErrorCodes.InvalidGameType, $"Game type {request.GameType} does not exist", context.Request.GetDisplayUrl(),   Enum.GetNames<GameType>());
                return TypedResults.BadRequest(error);
            }
            return TypedResults.Created($"/games/{game.Id}", game.ToCreateGameResponse());
        })
        .WithName("CreateGame")
        .WithSummary("Creates and starts a game")
        .WithOpenApi(op =>
        {
            op.RequestBody.Description = "The game type and the player name of the game to create";
            return op;
        });
```

Using the group, specify MapPatch to update the game with a move:

```csharp
       // Update the game resource with a move
       group.MapPatch("/{id:guid}", async Task<Results<Ok<UpdateGameResponse>, NotFound, BadRequest<GameError>>> (
           Guid id,
           UpdateGameRequest request,
           IGamesService gameService,
           HttpContext context,
           CancellationToken cancellationToken) =>
       {
           if (request.GuessPegs == null && !request.End)
           {
               return TypedResults.BadRequest(new GameError(ErrorCodes.InvalidMove, "End the game or set guesses", context.Request.GetDisplayUrl()));
           }
           try
           {
               if (request.End)
               {
                   Game? game = await gameService.EndGameAsync(id, cancellationToken);
                   if (game is null)
                       return TypedResults.NotFound();
                   return TypedResults.Ok(game.ToUpdateGameResponse());
               }
               else
               {
                   // guess pegs could only be null if request.End is true, checked above
                   (Game game, Move move) = await gameService.SetMoveAsync(id, request.GameType.ToString(), request.GuessPegs!, request.MoveNumber, cancellationToken);
                   return TypedResults.Ok(game.ToUpdateGameResponse(move.KeyPegs));
               }
           }
           catch (ArgumentException ex) when (ex.HResult is >= 4200 and <= 4500)
           {
               string url = context.Request.GetDisplayUrl();
               return ex.HResult switch
               {
                   4200 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuessNumber, "Invalid number of guesses received", url)),
                   4300 => TypedResults.BadRequest(new GameError(ErrorCodes.UnexpectedMoveNumber, "Unexpected move number received", url)),
                   > 4400 and < 4490 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuess, "Invalid guess values received!", url)),
                   _ => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidMove,"Invalid move received!", url))
               };
           }
           catch (CodebreakerException ex)
           {
               string url = context.Request.GetDisplayUrl();
               return ex.Code switch
               {
                   CodebreakerExceptionCodes.GameNotFound => TypedResults.NotFound(),
                   CodebreakerExceptionCodes.UnexpectedGameType => TypedResults.BadRequest(new GameError(ErrorCodes.UnexpectedGameType, "The game type specified with the move does not match the type of the running game", url)),
                   CodebreakerExceptionCodes.GameNotActive => TypedResults.BadRequest(new GameError(ErrorCodes.GameNotActive, "The game already ended", url)),
                   _ => TypedResults.BadRequest(new GameError("Unexpected", "Game error", url))
               };
           }
       })
       .WithName("SetMove")
       .WithSummary("End the game or set a move")
       .WithOpenApi(op =>
       {
           op.Parameters[0].Description = "The id of the game to set a move";
           op.RequestBody.Description = "The data for creating the move";
           return op;
       });
```

Define the `MapGet` method to return a single game:

```csharp
        // Get game by id
        group.MapGet("/{id:guid}", async Task<Results<Ok<Game>, NotFound>> (
            Guid id,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            Game? game = await gameService.GetGameAsync(id, cancellationToken);

            if (game is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(game);
        })
        .WithName("GetGame")
        .WithSummary("Gets a game by the given id")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to get";
            return op;
        });
```

## Configure the Middleware (ApplictationServices.cs)

```csharp
public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "SqlServer":
                break;
            case "Cosmos":
                break;
            case "DistributedMemory":
                break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();

        builder.AddRedisDistributedCache("redis");
    }
}
``` 

And with the top-level statements DI container configuration

```csharp
builder.AddApplicationServices();
```

And the middleware

```
app.MapGameEndpoints();
```

## HTTP File

Create an HTTP file to test the API:

```http
@Codebreaker.GameAPIs_HostAddress = http://localhost:9400
@ContentType = application/json

### Create a game
POST {{Codebreaker.GameAPIs_HostAddress}}/games/
Content-Type: {{ContentType}}

{
  "gameType": "Game6x4",
  "playerName": "test"
}

### Set a move

@id = 77822526-76eb-4a8f-8186-461226168170

PATCH {{Codebreaker.GameAPIs_HostAddress}}/games/{{id}}
Content-Type: {{ContentType}}

{
  "gameType": "Game6x4",
  "playerName": "test",
  "moveNumber": 1,
  "guessPegs": [
    "Red",
    "Green",
    "Blue",
    "Yellow"
  ]
}

### Get game information

GET {{Codebreaker.GameAPIs_HostAddress}}/games/{{id}}
```

## Run the service

Run the service locally and test it with the HTTP file.
