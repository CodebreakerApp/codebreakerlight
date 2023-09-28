using CodeBreaker.Shared.Models.Api;

namespace CodeBreaker.Services;
public interface IGameReportClient
{
    Task<GetGameResponse?> GetGameAsync(Guid id);

    Task<GetGamesResponse?> GetGamesAsync(DateTime? date);
}
