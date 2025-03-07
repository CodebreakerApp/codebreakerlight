var builder = DistributedApplication.CreateBuilder(args);

string dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var gameApis = builder.AddProject<Projects.GamesApi>("gamesapi")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStore);

var bot = builder.AddProject<Projects.Bot>("bot")
    .WithReference(gameApis)
    .WaitFor(gameApis);

if (dataStore == "SqlServer")
{
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume();
    var sqlDB = sqlServer
        .AddDatabase("CodebreakerSql", "codebreaker");

    gameApis
        .WithReference(sqlDB)
        .WaitFor(sqlServer);
}
else if (dataStore == "Cosmos")
{
    // #if DEBUG
    // This is another option using the local installed emulator
    // with this you need to start the Azure Cosmos emulator on your system, and create a Codebreaker database in this emulator (see information in the readme file)
    // var cosmosDB = builder.AddConnectionString("codebreakercosmos");

    //var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
    //    .RunAsPreviewEmulator(p =>
    //        p.WithDataExplorer()
    //            .WithDataVolume()
    //            .WithLifetime(ContainerLifetime.Session));

    //var cosmosDB = cosmos
    //    .AddCosmosDatabase("codebreaker")
    //    .AddContainer("GamesV3", "/PartitionKey");

    // #else
    var cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
        .WithAccessKeyAuthentication();

    var cosmosDB = cosmos
        .AddCosmosDatabase("codebreaker")
        .AddContainer("GamesV3", "/PartitionKey");

    // #endif
    gameApis
        .WithReference(cosmos)
        .WaitFor(cosmos)
        .WithEnvironment(context =>
        {
            if (cosmos.Resource.IsEmulator || cosmos.Resource.UseAccessKeyAuthentication)
            {
                context.EnvironmentVariables["Aspire__Microsoft__EntityFrameworkCore__Cosmos__AppDbContext__ConnectionString"] = cosmos.Resource.ConnectionStringExpression;
            }
            else
            {
                context.EnvironmentVariables["Aspire__Microsoft__EntityFrameworkCore__Cosmos__AppDbContext__AccountEndpoint"] = cosmos.Resource.ConnectionStringExpression;
            }
        });
    // environment temporary workaround  https://github.com/dotnet/aspire/issues/7785#issuecomment-2686122073

}
else if (dataStore == "Postgres")
{
    var postgres = builder.AddPostgres("postgres")
        .WithPgAdmin()
        .AddDatabase("CodebreakerPostgres");

    gameApis
        .WithReference(postgres)
        .WaitFor(postgres);
}


builder.Build().Run();
