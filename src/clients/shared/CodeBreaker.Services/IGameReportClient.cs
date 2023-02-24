using CodeBreaker.Shared.Api;

namespace CodeBreaker.Services;
public interface IGameReportClient
{
    Task<GetGameResponse?> GetGameAsync(Guid id);

    Task<GetGamesResponse?> GetGamesAsync(DateTime? date);
}
