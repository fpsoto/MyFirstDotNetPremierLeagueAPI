using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class PlayerStatisticConfiguration : IEntityTypeConfiguration<PlayerStatistic>
{
    public void Configure(EntityTypeBuilder<PlayerStatistic> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Ignore(s => s.GoalsPerGame);
        builder.Ignore(s => s.AssistsPerGame);

        builder.HasIndex(s => new { s.PlayerId, s.SeasonId }).IsUnique();

        builder.HasOne(s => s.Season)
            .WithMany()
            .HasForeignKey(s => s.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("PlayerStatistics");
    }
}
