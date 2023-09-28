# CNinnovation.Codebreaker.ViewModels

This library contains the `GamesClient` class to communicate with the Codebreaker service, and model types that are used for the communication.

See https://github.com/codebreakerapp for more information on the complete solution.


## The ViewModels

TODO

| Class | Description |
|-------|-------------|
| GamePageViewModel | TODO |
| GameViewModel | TODO |
| MoveViewModel | TODO |
| InfoMessageViewModel | TODO |


The `GamesClient` class is the main class to use for communication. It contains the following methods:

| Method     | Description        |
|------------|--------------------|
| StartGameAsync | Start a new game |
| SetMoveAsync | Set guesses for a game move |
| GetGameAsync | Get a game by id with all details and moves |
| GetGamesAsync | Get a list of games with all details and moves (use the `GamesQuery` class to define the filter) |

In the constructor, inject the `HttpClient` class. You can use `Microsoft.Extensions.Http` to configure the `HttpClient` class.

## Model types

The following model types are used to return information about the game.

| Model type | Description |
|------------|-------------|
| Game | Contains the game id, the game status, the game moves and the game result |
| Move | Contains the move number, the guess and the result of the guess |
