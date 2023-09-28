using CodeBreaker.Shared.Models.Data;

namespace CodeBreaker.Shared.Models.Report;

public record GamesInformationDetail(DateTime Date)
{
    public List<Data.Game> Games { get; init; } = new List<Data.Game>();
}

public record GamesInfo(DateTime Time, string User, int NumberMoves, Guid Id);
