namespace PremierLeague.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITeamRepository Teams { get; }
    IPlayerRepository Players { get; }
    IMatchRepository Matches { get; }
    ILeagueStandingRepository Standings { get; }
    ISeasonRepository Seasons { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
