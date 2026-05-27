using PremierLeague.Domain.Entities;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class SeasonSeeder(AppDbContext context)
{
    public async Task<Season> SeedAsync(CancellationToken cancellationToken = default)
    {
        var season = Season.Create("2024-25",
            new DateOnly(2024, 8, 17),
            new DateOnly(2025, 5, 25),
            isActive: true);

        context.Seasons.Add(season);
        await context.SaveChangesAsync(cancellationToken);
        return season;
    }
}
