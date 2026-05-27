using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;

namespace PremierLeague.Domain.Interfaces;

public interface IMatchRepository : IRepository<Match>
{
    Task<Match?> GetByIdWithTeamsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetBySeasonAsync(Guid seasonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetRecentAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetUpcomingAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetByTeamAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetByMatchdayAsync(Guid seasonId, int matchday, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Match>> GetFilteredAsync(
        Guid seasonId, Guid? teamId, MatchStatus? status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetFilteredCountAsync(Guid seasonId, Guid? teamId, MatchStatus? status,
        CancellationToken cancellationToken = default);
}
