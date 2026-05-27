using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PremierLeague.Domain.Entities;

namespace PremierLeague.Infrastructure.Persistence.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(10);
        builder.HasIndex(s => s.Name).IsUnique();
        builder.HasIndex(s => s.IsActive);

        builder.ToTable("Seasons");
    }
}
