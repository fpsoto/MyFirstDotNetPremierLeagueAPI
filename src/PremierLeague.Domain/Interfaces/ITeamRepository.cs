using PremierLeague.Domain.Entities;

namespace PremierLeague.Domain.Interfaces;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team?> GetByIdWithPlayersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> GetAllWithStandingsAsync(Guid seasonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> GetBestDefenseAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Team>> GetBestAttackAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
