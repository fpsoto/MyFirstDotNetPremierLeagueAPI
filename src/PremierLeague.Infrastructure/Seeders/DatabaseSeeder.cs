using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

// Orchestrates all seeders in dependency order. Idempotent — safe to call on every startup.
public class DatabaseSeeder(
    AppDbContext context,
    SeasonSeeder seasonSeeder,
    TeamSeeder teamSeeder,
    PlayerSeeder playerSeeder,
    MatchSeeder matchSeeder,
    StandingSeeder standingSeeder,
    PlayerStatisticSeeder statisticSeeder,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (await context.Teams.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Database already seeded — skipping.");
            return;
        }

        logger.LogInformation("Starting database seeding...");

        var season = await seasonSeeder.SeedAsync(cancellationToken);
        var teams = await teamSeeder.SeedAsync(cancellationToken);
        var players = await playerSeeder.SeedAsync(teams, cancellationToken);
        var matches = await matchSeeder.SeedAsync(teams, season, cancellationToken);
        await standingSeeder.SeedAsync(teams, matches, season, cancellationToken);
        await statisticSeeder.SeedAsync(players, matches, season, cancellationToken);

        logger.LogInformation("Database seeding completed. Teams: {Teams}, Players: {Players}, Matches: {Matches}",
            teams.Count, players.Count, matches.Count);
    }
}
