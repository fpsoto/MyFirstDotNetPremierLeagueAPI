using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Persistence.Repositories;

public class SeasonRepository(AppDbContext context) : GenericRepository<Season>(context), ISeasonRepository
{
    public async Task<Season?> GetActiveAsync(CancellationToken cancellationToken = default)
        => await Context.Seasons
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IsActive, cancellationToken);

    public async Task<Season?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await Context.Seasons
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
}
