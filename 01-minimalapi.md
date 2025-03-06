# Part 1 - minimal APIs

> The game will be accessible via a REST API implemented with ASP.NET Core.

## Create the Project

Create a new *ASP.NET Core Web API* project using **minimal APIs**, named **Codebreaker.GameAPIs**.

Select these options:
- Authentication type: none
- Configure for HTTPS
- Enable OpenAPI support

Add these NuGet packages:
- CNinnovation.Codebreaker.BackendModels (Game and Move types)
- CNinnovation.Codebreaker.Analyzers (the analyzer to return results from a move)

## Create an in-memory games repository

From 01-Start, copy `GlobalUsings.cs` (or add namespaces as needed).

From 01-Start, copy the *Data* folder to the project. This contains the `GamesMemoryRepository`.


## Service-classes 

From 01-Start, copy the *Services* folder to the project. This contains these types:

- GamesFactory - a factory to create games
- IGamesService - a contract for GamesService
- GamesService - the implementation of `StartGameAsync` and `SetMoveAsync`.

Check these classes, and compile the application. Add namespaces as required.

> Register the IGamesRepository as a singleton in the DI container!!

## Endpoints

From 01-Start, copy the *Models* folder to the project. This contains model types used by the endpoint.

From 01-Start, copy the *Errors* folder to the project. This contains the `GameError` type with `ErrorCodes` to be returned from endpoint.

From 01-Start, copy the *Extensions* folder to the project. This contains extension methods used by the endpoint.

From 01-Start, copy the *Endpoints* folder to the project. This contains extension methods used by the endpoint.

> Register the IGamesService as transient in the DI container!!

Copy the `gameapis.http` file.

Map the game-endpoints.

## Test the solution!

Use the HTTP file, or add Swashbuckle for Swagger UI to test the application!
