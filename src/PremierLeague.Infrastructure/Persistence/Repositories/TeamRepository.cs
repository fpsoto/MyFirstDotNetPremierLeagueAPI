using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class TeamRepository(AppDbContext context) : GenericRepository<Team>(context), ITeamRepository
{
    public async Task<Team?> GetByIdWithPlayersAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Teams
            .Include(t => t.Players.Where(p => p.IsActive).OrderBy(p => p.ShirtNumber))
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Team>> GetAllWithStandingsAsync(Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Teams
            .Include(t => t.Standings.Where(s => s.SeasonId == seasonId))
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Team>> GetBestDefenseAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Teams
            .Include(t => t.Standings.Where(s => s.SeasonId == seasonId))
            .AsNoTracking()
            .Where(t => t.Standings.Any(s => s.SeasonId == seasonId))
            .OrderBy(t => t.Standings.First(s => s.SeasonId == seasonId).GoalsAgainst)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Team>> GetBestAttackAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Teams
            .Include(t => t.Standings.Where(s => s.SeasonId == seasonId))
            .AsNoTracking()
            .Where(t => t.Standings.Any(s => s.SeasonId == seasonId))
            .OrderByDescending(t => t.Standings.First(s => s.SeasonId == seasonId).GoalsFor)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Teams.AnyAsync(t => t.Name == name, cancellationToken);
}
