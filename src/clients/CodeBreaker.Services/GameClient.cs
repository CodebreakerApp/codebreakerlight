using CodeBreaker.Services.Authentication;
using CodeBreaker.Services.Authentication.Definitions;
using CodeBreaker.Shared.Models.Api;
using Microsoft.Extensions.Logging;

using System.Net.Http.Json;

namespace CodeBreaker.Services;

public class GameClient : IGameClient, IGameReportClient
{
    private readonly HttpClient _httpClient;

    private readonly ILogger _logger;

    private readonly IAuthService _authService;

    private readonly IAuthDefinition _authDefinition = new ApiServiceAuthDefinition();

    public GameClient(HttpClient httpClient, ILogger<GameClient> logger, IAuthService authService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authService = authService;
        _logger.LogInformation("Injected HttpClient with base address {uri} into GameClient", _httpClient.BaseAddress);
    }

    public async Task<CreateGameResponse> StartGameAsync(string username, string gameType)
    {
        await SetAuthentication();
        CreateGameRequest request = new(username, gameType);
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/games", request);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<CreateGameResponse>();
    }

    public async Task CancelGameAsync(Guid gameId)
    {
        await SetAuthentication();
        HttpResponseMessage responseMessage = await _httpClient.DeleteAsync($"/games/{gameId}?cancel=true");
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task<CreateMoveResponse> SetMoveAsync(Guid gameId, params string[] colorNames)
    {
        await SetAuthentication();
        CreateMoveRequest request = new CreateMoveRequest(colorNames.ToList());
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync($"/games/{gameId}/moves", request);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<CreateMoveResponse>();
    }

    public async Task<GetGamesResponse?> GetGamesAsync(DateTime? date)
    {
        await SetAuthentication();
        string requestUri = "/games";

        if (date is null)
            date = DateTime.Now.Date;

        requestUri = $"{requestUri}?date={date.Value.ToString("yyyy-MM-dd")}";
        _logger.LogInformation("Calling Codebreaker with {uri}", requestUri);

        return await _httpClient.GetFromJsonAsync<GetGamesResponse>(requestUri);
    }

    public async Task<GetGameResponse?> GetGameAsync(Guid id)
    {
        await SetAuthentication();
        string requestUri = $"/games/{id}";
        _logger.LogInformation("Calling Codebreaker with {uri}", requestUri);
        HttpResponseMessage responseMessage = await _httpClient.GetAsync(requestUri);
        responseMessage.EnsureSuccessStatusCode();
        return await responseMessage.Content.ReadFromJsonAsync<GetGameResponse>();
    }

    private async ValueTask SetAuthentication()
    {
        if (!_authService.IsAuthenticated)
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", await GetAccessTokenAsync());
    }

    private async Task<string> GetAccessTokenAsync() =>
        (await _authService.AquireTokenAsync(_authDefinition)).AccessToken;
}
