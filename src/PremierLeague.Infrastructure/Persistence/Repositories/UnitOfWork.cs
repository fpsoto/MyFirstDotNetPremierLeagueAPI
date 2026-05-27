using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class UnitOfWork(
    AppDbContext context,
    ITeamRepository teams,
    IPlayerRepository players,
    IMatchRepository matches,
    ILeagueStandingRepository standings,
    ISeasonRepository seasons) : IUnitOfWork
{
    public ITeamRepository Teams { get; } = teams;
    public IPlayerRepository Players { get; } = players;
    public IMatchRepository Matches { get; } = matches;
    public ILeagueStandingRepository Standings { get; } = standings;
    public ISeasonRepository Seasons { get; } = seasons;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public void Dispose() => context.Dispose();
}
