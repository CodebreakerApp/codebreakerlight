using System.Text.Json;
using CodeBreaker.Services.Authentication;
using CodeBreaker.Services.Authentication.Definitions;
using CodeBreaker.Services.EventArguments;
using CodeBreaker.Shared.Models.Live;
using CodeBreaker.Shared.Models.Live.EventPayloads;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CodeBreaker.Shared.Models.Live.LiveEventNames;

namespace CodeBreaker.Services;

public class LiveClientOptions
{
    public string LiveBase { get; set; } = string.Empty;
}

public class LiveClient
{
    private readonly ILogger _logger;

    private readonly HubConnection _hubConnection;

    private readonly IAuthService _authService;

    private readonly IAuthDefinition _liveAuthDefinition = new LiveServiceAuthDefinition();

    public event EventHandler<OnGameEventArgs>? OnGameEvent;

    public event EventHandler<OnMoveEventArgs>? OnMoveEvent;

    private readonly Dictionary<string, Action<LiveHubArgs>> _eventHandlers = new ();

    public LiveClient(ILogger<LiveClient> logger, IOptions<LiveClientOptions> options, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
        _hubConnection = BuildHubConnection(options.Value.LiveBase);
        InitializeEventHandlers();
        _hubConnection.On<LiveHubArgs>("gameEvent", OnRemoteEvent);
    }

    private HubConnection BuildHubConnection(string liveBase) =>
        new HubConnectionBuilder()
            .WithUrl(liveBase, options =>
            {
                options.AccessTokenProvider = async () => (await _authService.AquireTokenAsync(_liveAuthDefinition)).AccessToken;
            })
            .WithAutomaticReconnect()
            .ConfigureLogging(x =>
            {
                x.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

    public Task StartAsync(CancellationToken token = default)
    {
        return _hubConnection.StartAsync(token);
    }

    public IAsyncEnumerable<LiveHubArgs> SubscribeToEventsAsync(CancellationToken token = default)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
            throw new InvalidOperationException("The SignalR-Client has to be started before");

        return _hubConnection.StreamAsync<LiveHubArgs>("SubscribeToGameEvents", token);
    }

    private void OnRemoteEvent(LiveHubArgs args)
    {
        if (args is null || args.Name is null)
            throw new ArgumentNullException(nameof(args));

        if (!_eventHandlers.TryGetValue(args.Name, out Action<LiveHubArgs>? action))
            throw new ArgumentOutOfRangeException(nameof(args.Name), "No EventHandler for this eventName found.");

        action(args);
    }

    private void InitializeEventHandlers()
    {
        _eventHandlers.Add(GameCreatedEventName, eventData => Handle<GameCreatedPayload, OnGameEventArgs>(eventData, () => OnGameEvent));
        _eventHandlers.Add(MoveCreatedEventName, eventData => Handle<MoveCreatedPayload, OnMoveEventArgs>(eventData, () => OnMoveEvent));
    }

    private void Handle<T, TArgs>(LiveHubArgs liveHubArgs, Func<EventHandler<TArgs>?> eventHandler)
        where TArgs : ILiveEventArgs<T?>, new()
    {
        string? payloadString = liveHubArgs.Data.ToString();

        if (payloadString is null)
        {
            _logger.LogInformation("EventPayload is null. Skipping event.");
            return;
        }

        T? data = JsonSerializer.Deserialize<T>(payloadString);

        if (data is null)
        {
            _logger.LogInformation("EventData is null. Skipping event.");
            return;
        }

        TArgs args = new TArgs()
        {
            Data = data
        };
        eventHandler()?.Invoke(this, args);
    }
}
