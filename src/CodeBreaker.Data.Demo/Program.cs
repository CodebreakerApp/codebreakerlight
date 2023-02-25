using Azure.Identity;
using CodeBreaker.Data;
using CodeBreaker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using static CodeBreaker.Shared.Colors;

string cosmosEndpoint = "https://codebreaker-db-account-563722.documents.azure.com:443/";
string databaseName = "codebreaker-db";
//DefaultAzureCredential azureCredential = new();
AzureCliCredential azureCredential = new();

var contextOptions = new DbContextOptionsBuilder<CodeBreakerContext>()
    .UseCosmos(cosmosEndpoint, azureCredential, databaseName)
    .Options;
var dummyLogger = new LoggerFactory().CreateLogger<CodeBreakerContext>();
using var context = new CodeBreakerContext(contextOptions, dummyLogger);

Game game = new() {
    GameId = Guid.NewGuid(),
    Code = new string[] { Red, Blue, Black, White },
    Username = "Sebastian",
    Start = DateTime.Now,
    Colors = Array.Empty<string>(),
    MaxMoves = 12,
};

Console.WriteLine($"Creating game: {game}");

context.Games.Add(game);
await context.SaveChangesAsync();

Console.WriteLine("Created game");
