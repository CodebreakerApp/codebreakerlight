using Azure.Identity;
using CodeBreaker.APIs.Endpoints;
using CodeBreaker.APIs.Services;
using CodeBreaker.Data;
using CodeBreaker.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

#if DEBUG
AzureCliCredential azureCredential = new();
#else
DefaultAzureCredential azureCredential = new();
#endif

var builder = WebApplication.CreateBuilder(args);

// DI
string configEndpoint = builder.Configuration.GetRequired("AzureAppConfigurationEndpoint");
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(configEndpoint), azureCredential)
        .Select(KeyFilter.Any, LabelFilter.Null)
        .Select(KeyFilter.Any, builder.Environment.EnvironmentName);
    //.ConfigureKeyVault(vault => vault.SetCredential(azureCredential));
});
builder.Services.AddAzureAppConfiguration();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration.GetRequired("ApiService:ApplicationInsights:ConnectionString");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ICodeBreakerRepository, CodeBreakerContext>(options =>
{
    string accountEndpoint = builder.Configuration.GetRequired("ApiService:Cosmos:AccountEndpoint");
    string databaseName = builder.Configuration.GetRequired("ApiService:Cosmos:DatabaseName");
    options.UseCosmos(accountEndpoint, azureCredential, databaseName);
});

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMoveService, MoveService>();

var app = builder.Build();

// Middlewares

app.UseSwagger();
app.UseSwaggerUI();

app.MapGameEndpoints();

app.Run();