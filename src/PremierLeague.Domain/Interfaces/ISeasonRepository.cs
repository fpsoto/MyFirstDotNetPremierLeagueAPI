using PremierLeague.Domain.Entities;

namespace PremierLeague.Domain.Interfaces;

public interface ISeasonRepository : IRepository<Season>
{
    Task<Season?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Season?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
