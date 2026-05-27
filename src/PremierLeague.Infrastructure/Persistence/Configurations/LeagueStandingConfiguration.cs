using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class LeagueStandingConfiguration : IEntityTypeConfiguration<LeagueStanding>
{
    public void Configure(EntityTypeBuilder<LeagueStanding> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Ignore(s => s.GoalDifference);

        builder.HasIndex(s => new { s.TeamId, s.SeasonId }).IsUnique();

        builder.HasOne(s => s.Team)
            .WithMany(t => t.Standings)
            .HasForeignKey(s => s.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Season)
            .WithMany(se => se.Standings)
            .HasForeignKey(s => s.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("LeagueStandings");
    }
}
