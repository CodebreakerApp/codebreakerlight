# Part 2 - Add .NET Aspire

## Add SQL Server with EF Core to the Games API

Add the EF Core SQL Server and Azure Cosmos DB components to the Games API.

See: 
- [SQL Server EF Core Component](https://learn.microsoft.com/en-us/dotnet/aspire/database/sql-server-entity-framework-component?tabs=dotnet-cli)
- [Azure Cosmos DB EF Core Component](https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-entity-framework-component?tabs=dotnet-cli)

Add the NuGet Packages for Codebreaker Cosmos and SQL Server to the games API:

- https://www.nuget.org/packages/CNinnovation.Codebreaker.Cosmos
- https://www.nuget.org/packages/CNinnovation.Codebreaker.SqlServer


Add the configuration of the DBContext types with the ApplicationServices class:

```csharp
public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.AddSqlServerDbContext<GamesSqlServerContext>("CodebreakerSql",
                configureSettings: settings =>
                {
                    settings.Metrics = true;
                    settings.Tracing = true;
                },
                configureDbContextOptions: options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });
            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesSqlServerContext>>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.AddCosmosDbContext<GamesCosmosContext>("GamesCosmosConnection", "codebreaker",
                configureSettings: settings =>
                {
                    settings.Metrics = true;
                    settings.Tracing = true;
                },
                configureDbContextOptions: options =>
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        static void ConfigureDistributedMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddTransient<IGamesRepository, DistributedMemoryGamesRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            case "DistributedMemory":
                ConfigureDistributedMemory(builder);
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

- Configure the SQL Server Docker container with the App host
- Configure Azure Cosmos DB with the App host

See
- https://learn.microsoft.com/en-us/dotnet/aspire/database/sql-server-component?tabs=dotnet-cli
- https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-entity-framework-component
- https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/persist-data-volumes
