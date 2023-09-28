using CodeBreaker.Shared.Models.Data;

namespace CodeBreaker.Shared.Models.Live.EventPayloads;

public record class MoveCreatedPayload(Guid GameId, Move Move);
