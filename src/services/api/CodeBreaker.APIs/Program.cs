using Azure.Core.Diagnostics;
using Azure.Identity;

using CodeBreaker.APIs.Endpoints;
using CodeBreaker.APIs.Services;
using CodeBreaker.APIs.Utilities;
using CodeBreaker.Data;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

#if DEBUG

// Setup a listener to monitor logged events.
using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();

DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions
{
    Diagnostics =
    {
        LoggedHeaderNames = { "x-ms-request-id" },
        LoggedQueryParameters = { "api-version" },
        IsLoggingContentEnabled = true,
        IsAccountIdentifierLoggingEnabled = true
    }
};

// DefaultAzureCredential azureCredential = new(options);
AzureCliCredential azureCredential = new();
#else
DefaultAzureCredential azureCredential = new();
#endif

var builder = WebApplication.CreateBuilder(args);

// DI
string configEndpoint = builder.Configuration["AzureAppConfigurationEndpoint"] ?? throw new InvalidOperationException();
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
    options.ConnectionString = builder.Configuration["ApiService:ApplicationInsights:ConnectionString"] ?? throw new InvalidOperationException();
});
builder.Services.AddSingleton<ITelemetryInitializer, ApplicationInsightsTelemetryInitializer>();

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ICodeBreakerRepository, CodeBreakerContext>(options =>
{
    string accountEndpoint = builder.Configuration["ApiService:Cosmos:AccountEndpoint"] ?? throw new InvalidOperationException();
    string databaseName = builder.Configuration["ApiService:Cosmos:DatabaseName"] ?? "Thrive2023";
    options.UseCosmos(accountEndpoint, azureCredential, databaseName);
});

builder.Services.AddScoped<IGameService, GameService>();


var app = builder.Build();

// Middleware

app.UseCors(config =>
{
    config.AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin();
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapGameEndpoints();

app.Run();