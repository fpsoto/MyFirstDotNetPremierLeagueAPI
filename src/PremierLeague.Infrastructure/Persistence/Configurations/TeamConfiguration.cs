using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.ShortName).IsRequired().HasMaxLength(5);
        builder.Property(t => t.Stadium).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Coach).IsRequired().HasMaxLength(100);
        builder.Property(t => t.City).IsRequired().HasMaxLength(100);
        builder.Property(t => t.PrimaryColor).IsRequired().HasMaxLength(30);

        builder.HasIndex(t => t.Name).IsUnique();

        builder.HasMany(t => t.Players)
            .WithOne(p => p.Team)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.HomeMatches)
            .WithOne(m => m.HomeTeam)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.AwayMatches)
            .WithOne(m => m.AwayTeam)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Teams");
    }
}
