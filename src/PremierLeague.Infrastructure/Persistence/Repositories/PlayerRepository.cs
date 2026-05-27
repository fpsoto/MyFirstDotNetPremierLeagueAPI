using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class PlayerRepository(AppDbContext context) : GenericRepository<Player>(context), IPlayerRepository
{
    public async Task<Player?> GetByIdWithStatisticsAsync(Guid id, Guid seasonId, CancellationToken cancellationToken = default)
        => await Context.Players
            .Include(p => p.Team)
            .Include(p => p.Statistic)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.Statistic != null && p.Statistic.SeasonId == seasonId, cancellationToken)
            ?? await Context.Players
                .Include(p => p.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Player>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
        => await Context.Players
            .AsNoTracking()
            .Where(p => p.TeamId == teamId && p.IsActive)
            .OrderBy(p => p.ShirtNumber)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Player>> GetTopScorersAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Players
            .Include(p => p.Team)
            .Include(p => p.Statistic)
            .AsNoTracking()
            .Where(p => p.Statistic != null && p.Statistic.SeasonId == seasonId && p.Statistic.Goals > 0)
            .OrderByDescending(p => p.Statistic!.Goals)
            .ThenByDescending(p => p.Statistic!.Assists)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Player>> GetTopAssistsAsync(Guid seasonId, int take, CancellationToken cancellationToken = default)
        => await Context.Players
            .Include(p => p.Team)
            .Include(p => p.Statistic)
            .AsNoTracking()
            .Where(p => p.Statistic != null && p.Statistic.SeasonId == seasonId && p.Statistic.Assists > 0)
            .OrderByDescending(p => p.Statistic!.Assists)
            .ThenByDescending(p => p.Statistic!.Goals)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Player>> GetFilteredAsync(
        Guid? teamId, PlayerPosition? position, string? nationality,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Players
            .Include(p => p.Team)
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (teamId.HasValue) query = query.Where(p => p.TeamId == teamId.Value);
        if (position.HasValue) query = query.Where(p => p.Position == position.Value);
        if (!string.IsNullOrWhiteSpace(nationality))
            query = query.Where(p => p.Nationality.Contains(nationality));

        return await query
            .OrderBy(p => p.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFilteredCountAsync(Guid? teamId, PlayerPosition? position, string? nationality,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Players.Where(p => p.IsActive);

        if (teamId.HasValue) query = query.Where(p => p.TeamId == teamId.Value);
        if (position.HasValue) query = query.Where(p => p.Position == position.Value);
        if (!string.IsNullOrWhiteSpace(nationality))
            query = query.Where(p => p.Nationality.Contains(nationality));

        return await query.CountAsync(cancellationToken);
    }
}
