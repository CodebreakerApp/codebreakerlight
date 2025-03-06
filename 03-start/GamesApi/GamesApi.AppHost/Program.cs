var builder = DistributedApplication.CreateBuilder(args);

var gamesApi = builder.AddProject<Projects.GamesApi>("gamesapi");

var bot = builder.AddProject<Projects.Bot>("bot")
    .WithReference(gamesApi)
    .WaitFor(gamesApi);

builder.Build().Run();
