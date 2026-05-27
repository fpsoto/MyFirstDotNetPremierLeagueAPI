using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class LeagueStandingRepository(AppDbContext context) : GenericRepository<LeagueStanding>(context), ILeagueStandingRepository
{
    public async Task<IReadOnlyList<LeagueStanding>> GetBySeasonAsync(Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Standings
            .Include(s => s.Team)
            .AsNoTracking()
            .Where(s => s.SeasonId == seasonId)
            .OrderBy(s => s.Position)
            .ToListAsync(cancellationToken);

    public async Task<LeagueStanding?> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Standings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TeamId == teamId && s.SeasonId == seasonId, cancellationToken);
}
