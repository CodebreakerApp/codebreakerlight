using CodeBreaker.APIs.Extensions;
using CodeBreaker.APIs.Services;
using CodeBreaker.Data.Models;
using CodeBreaker.Shared.Api;
using CodeBreaker.Shared.Exceptions;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CodeBreaker.APIs.Endpoints;

internal static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games").WithTags(nameof(Game));

        group.MapGet("/", (
            [FromQuery] DateTime date,
            [FromServices] IGameService gameService
        ) =>
        {
            var games = gameService
                .GetByDateAsync(date)
                .Select(x => x.ToDto());

            return TypedResults.Ok(new GetGamesResponse(games.ToEnumerable())); // For production we should pass the IAsyncEnumerable instead of converting it to IEnumerable
        })
        .WithName("GetGames")
        .WithSummary("Get games by given date")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The date of the games";
            return x;
        });

        // Get game by id
        group.MapGet("/{gameId:guid}", async Task<Results<Ok<GetGameResponse>, NotFound>> (
            [FromRoute] Guid gameId,
            [FromServices] IGameService gameService
        ) =>
        {
            Game? game = await gameService.GetAsync(gameId);

            if (game is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(new GetGameResponse(game.ToDto()));
        })
        .WithName("GetGame")
        .WithSummary("Gets a game by the given id")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The id of the game to get";
            return x;
        });

        // Create game
        group.MapPost("/", async (
            [FromBody] CreateGameRequest req,
            [FromServices] IGameService gameService) =>
        {
            Game game = await gameService.CreateAsync(req.Username);
            return TypedResults.Created($"/games/{game.GameId}", new CreateGameResponse(game.ToDto()));
        })
        .WithName("CreateGame")
        .WithSummary("Creates and starts a game")
        .WithOpenApi(x =>
        {
            x.RequestBody.Description = "The data of the game to create";
            return x;
        });

        // Create move for game
        group.MapPost("/{gameId:guid}/moves", async Task<Results<Ok<CreateMoveResponse>, NotFound, BadRequest<string>>>(
            [FromRoute] Guid gameId,
            [FromBody] CreateMoveRequest req,
            [FromServices] IMoveService moveService) =>
        {
            Game game;
            Move move = new()
            {
                MoveNumber = 0,
                GuessPegs = req.GuessPegs,
            };

            try
            {
                game = await moveService.CreateMoveAsync(gameId, move);
            }
            catch (GameNotFoundException)
            {
                return TypedResults.NotFound();
            }

            KeyPegs? keyPegs = game.GetLastKeyPegsOrDefault();

            if (keyPegs is null)
                return TypedResults.BadRequest("Could not get keyPegs");

            return TypedResults.Ok(new CreateMoveResponse(((KeyPegs)keyPegs).ToDto(), game.Ended, game.Won));
        })
        .WithName("CreateMove")
        .WithSummary("Creates a move for the game with the given id")
        .WithOpenApi(x =>
        {
            x.Parameters[0].Description = "The id of the game to create a move for";
            x.RequestBody.Description = "The data for creating the move";
            return x;
        });
    }
}
