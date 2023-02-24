using CodeBreaker.Shared.Api;
using Microsoft.Extensions.Logging;

using System.Net.Http.Json;

namespace CodeBreaker.Services;

public class GameClient : IGameClient, IGameReportClient
{
    private readonly HttpClient _httpClient;

    private readonly ILogger _logger;

    public GameClient(HttpClient httpClient, ILogger<GameClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("Injected HttpClient with base address {uri} into GameClient", _httpClient.BaseAddress);
    }

    public async Task<CreateGameResponse> StartGameAsync(string username)
    {
        CreateGameRequest request = new(username);
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/games", request);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<CreateGameResponse>();
    }

    public async Task<CreateMoveResponse> SetMoveAsync(Guid gameId, params string[] colorNames)
    {
        CreateMoveRequest request = new(colorNames.ToList());
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync($"/games/{gameId}/moves", request);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<CreateMoveResponse>();
    }

    public async Task<GetGamesResponse?> GetGamesAsync(DateTime? date)
    {
        string requestUri = "/games";

        date ??= DateTime.Now.Date;

        requestUri = $"{requestUri}?date={date.Value:yyyy-MM-dd}";
        _logger.LogInformation("Calling Codebreaker with {uri}", requestUri);

        return await _httpClient.GetFromJsonAsync<GetGamesResponse>(requestUri);
    }

    public async Task<GetGameResponse?> GetGameAsync(Guid id)
    {
        string requestUri = $"/games/{id}";
        _logger.LogInformation("Calling Codebreaker with {uri}", requestUri);
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(requestUri);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<GetGameResponse>();
    }
}
