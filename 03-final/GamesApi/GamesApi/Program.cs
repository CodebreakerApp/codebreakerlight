var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.AddApplicationServices();
//builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
//builder.Services.AddTransient<IGamesService, GamesService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.CreateOrUpdateDatabaseAsync();

app.MapGameEndpoints();

app.Run();
