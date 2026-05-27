using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;
using PremierLeague.Infrastructure.Persistence.Repositories;
using PremierLeague.Infrastructure.Seeders;

namespace PremierLeague.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)));

        // Repositories registered as scoped to align with DbContext lifetime
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ILeagueStandingRepository, LeagueStandingRepository>();
        services.AddScoped<ISeasonRepository, SeasonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Seeders
        services.AddScoped<SeasonSeeder>();
        services.AddScoped<TeamSeeder>();
        services.AddScoped<PlayerSeeder>();
        services.AddScoped<MatchSeeder>();
        services.AddScoped<StandingSeeder>();
        services.AddScoped<PlayerStatisticSeeder>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
