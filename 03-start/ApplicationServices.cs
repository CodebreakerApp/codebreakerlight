using Codebreaker.Data.Cosmos;
using Codebreaker.Data.Postgres;
using Codebreaker.Data.SqlServer;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContextPool<IGamesRepository, GamesSqlServerContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("CodebreakerSql") ?? throw new InvalidOperationException("Could not read SQL Server connection string");
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichSqlServerDbContext<GamesSqlServerContext>();
        }

        static void ConfigurePostgres(IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("CodebreakerPostgres") ?? throw new InvalidOperationException("Could not read SQL Server connection string");

            builder.Services.AddNpgsqlDataSource(connectionString, dataSourceBuilder =>
            {
                dataSourceBuilder.EnableDynamicJson(); 
            });

            builder.Services.AddDbContextPool<IGamesRepository, GamesPostgresContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            builder.EnrichNpgsqlDbContext<GamesPostgresContext>();
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<IGamesRepository, GamesCosmosContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("codebreakercosmos") ?? throw new InvalidOperationException("Could not read Cosmos connection string");
                options.UseCosmos(connectionString, "codebreaker", cosmosOptions =>
                {
                    cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(1));
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.EnrichCosmosDbContext<GamesCosmosContext>(settings =>
            {

            });
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            case "Postgres":
                ConfigurePostgres(builder);
                break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();
    }

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        var dataStore = app.Configuration["DataStore"] ?? "InMemory";
        if (dataStore == "SqlServer")
        {
            try
            {
                using var scope = app.Services.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesSqlServerContext context)
                {
                    await context.Database.MigrateAsync();
                    app.Logger.LogInformation("SQL Server database updated");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }
        else if (dataStore == "Postgres")
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesPostgresContext context)
                {
                    // TODO: migrations might be done in another step
                    // for now, just ensure the database is created
                    await context.Database.EnsureCreatedAsync();
                    app.Logger.LogInformation("PostgreSQL database created");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }
        else if (dataStore == "Cosmos")
        {
            // with Aspire 9.1, the database and container is created from the app model
            //try
            //{
            //    using var scope = app.Services.CreateScope();
            //    var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
            //    if (repo is GamesCosmosContext context)
            //    {
            //        bool created = await context.Database.EnsureCreatedAsync();
            //        app.Logger.LogInformation("Cosmos database created: {created}", created);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    app.Logger.LogError(ex, "Error updating database");
            //    throw;
            //}
        }
    }
}