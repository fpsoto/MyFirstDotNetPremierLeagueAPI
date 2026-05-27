using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;

namespace PremierLeague.Domain.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByIdWithStatisticsAsync(Guid id, Guid seasonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetTopScorersAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetTopAssistsAsync(Guid seasonId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetFilteredAsync(
        Guid? teamId, PlayerPosition? position, string? nationality,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetFilteredCountAsync(Guid? teamId, PlayerPosition? position, string? nationality,
        CancellationToken cancellationToken = default);
}
