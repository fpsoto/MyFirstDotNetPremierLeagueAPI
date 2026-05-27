using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(60);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(60);
        builder.Property(p => p.Nationality).IsRequired().HasMaxLength(60);
        builder.Property(p => p.MarketValueMillions).HasColumnType("decimal(10,2)");
        builder.Property(p => p.Position).IsRequired();

        builder.Ignore(p => p.FullName);

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Statistic)
            .WithOne(s => s.Player)
            .HasForeignKey<PlayerStatistic>(s => s.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Players");
    }
}
