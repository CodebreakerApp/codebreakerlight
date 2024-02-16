var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GamesAPI>("gamesapi");

builder.Build().Run();
