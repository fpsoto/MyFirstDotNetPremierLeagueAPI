using PremierLeague.Domain.Entities;

namespace PremierLeague.Domain.Interfaces;

public interface ILeagueStandingRepository : IRepository<LeagueStanding>
{
    Task<IReadOnlyList<LeagueStanding>> GetBySeasonAsync(Guid seasonId, CancellationToken cancellationToken = default);
    Task<LeagueStanding?> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default);
}
