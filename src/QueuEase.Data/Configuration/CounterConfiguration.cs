using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class CounterConfiguration : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(DataConstants.CounterNameMaxLength);

        // Two counters in one location cannot have the same name ("Гише 1" twice).
        builder.HasIndex(c => new { c.LocationId, c.Name }).IsUnique();

        // Lets CounterService point to (Id, LocationId); see ServiceConfiguration.
        builder.HasAlternateKey(c => new { c.Id, c.LocationId });

        builder.HasOne(c => c.Location)
            .WithMany(l => l.Counters)
            .HasForeignKey(c => c.LocationId);

        builder.HasOne(c => c.CurrentEmployee)
            .WithMany()
            .HasForeignKey(c => c.CurrentEmployeeId);

        // An employee can work at only one counter at a time. Closed counters (null) are not checked.
        builder.HasIndex(c => c.CurrentEmployeeId)
            .IsUnique()
            .HasFilter("[CurrentEmployeeId] IS NOT NULL");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Counters_Name", "LEN([Name]) > 0");
            // Nobody can work at a deactivated counter.
            t.HasCheckConstraint("CK_Counters_InactiveHasNoEmployee", "[IsActive] = 1 OR [CurrentEmployeeId] IS NULL");
        });
    }
}
