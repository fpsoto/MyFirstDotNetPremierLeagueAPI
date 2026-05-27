using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class MatchRepository(AppDbContext context) : GenericRepository<Match>(context), IMatchRepository
{
    public async Task<Match?> GetByIdWithTeamsAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Match>> GetBySeasonAsync(Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId)
            .OrderBy(m => m.Matchday).ThenBy(m => m.MatchDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Match>> GetRecentAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId && m.Status == MatchStatus.Completed)
            .OrderByDescending(m => m.MatchDate)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Match>> GetUpcomingAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId && m.Status == MatchStatus.Scheduled && m.MatchDate >= DateTime.UtcNow)
            .OrderBy(m => m.MatchDate)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Match>> GetByTeamAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId && (m.HomeTeamId == teamId || m.AwayTeamId == teamId))
            .OrderBy(m => m.Matchday)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Match>> GetByMatchdayAsync(Guid seasonId, int matchday, CancellationToken cancellationToken = default)
        => await Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId && m.Matchday == matchday)
            .OrderBy(m => m.MatchDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Match>> GetFilteredAsync(
        Guid seasonId, Guid? teamId, MatchStatus? status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Matches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .AsNoTracking()
            .Where(m => m.SeasonId == seasonId);

        if (teamId.HasValue)
            query = query.Where(m => m.HomeTeamId == teamId.Value || m.AwayTeamId == teamId.Value);
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        return await query
            .OrderByDescending(m => m.MatchDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFilteredCountAsync(Guid seasonId, Guid? teamId, MatchStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Matches.Where(m => m.SeasonId == seasonId);

        if (teamId.HasValue)
            query = query.Where(m => m.HomeTeamId == teamId.Value || m.AwayTeamId == teamId.Value);
        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        return await query.CountAsync(cancellationToken);
    }
}
