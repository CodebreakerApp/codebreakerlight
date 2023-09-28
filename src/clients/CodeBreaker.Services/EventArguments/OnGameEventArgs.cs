using CodeBreaker.Shared.Models.Live.EventPayloads;

namespace CodeBreaker.Services.EventArguments;

public class OnGameEventArgs : EventArgs, ILiveEventArgs<GameCreatedPayload?>
{
    public GameCreatedPayload? Data { get; init; }
}
