using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.Matchday).IsRequired();
        builder.Property(m => m.MatchDate).IsRequired();

        builder.HasIndex(m => new { m.SeasonId, m.Matchday });

        builder.HasOne(m => m.Season)
            .WithMany(s => s.Matches)
            .HasForeignKey(m => m.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing FKs need explicit no-action to avoid multiple cascade paths
        builder.HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable("Matches");
    }
}
