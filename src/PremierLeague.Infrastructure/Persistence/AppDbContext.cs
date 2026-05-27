using Microsoft.EntityFrameworkCore;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<LeagueStanding> Standings => Set<LeagueStanding>();
    public DbSet<PlayerStatistic> PlayerStatistics => Set<PlayerStatistic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
