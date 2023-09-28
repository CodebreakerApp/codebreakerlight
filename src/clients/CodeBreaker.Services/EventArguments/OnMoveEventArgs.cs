using CodeBreaker.Shared.Models.Live.EventPayloads;

namespace CodeBreaker.Services.EventArguments;

public class OnMoveEventArgs : EventArgs, ILiveEventArgs<MoveCreatedPayload?>
{
    public MoveCreatedPayload? Data { get; init; }
}
