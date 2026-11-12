using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class DailyCounterConfiguration : IEntityTypeConfiguration<DailyCounter>
{
    public void Configure(EntityTypeBuilder<DailyCounter> builder)
    {
        builder.HasOne(dc => dc.Location)
            .WithMany()
            .HasForeignKey(dc => dc.LocationId);

        // (ServiceId, LocationId): the service must belong to the same location.
        builder.HasOne(dc => dc.Service)
            .WithMany()
            .HasForeignKey(dc => new { dc.ServiceId, dc.LocationId })
            .HasPrincipalKey(s => new { s.Id, s.LocationId });

        // One row per location, service and day.
        builder.HasIndex(dc => new { dc.LocationId, dc.ServiceId, dc.Date }).IsUnique();

        // 0 means "nothing issued yet today"; the first ticket gets 1.
        builder.ToTable(t => t.HasCheckConstraint("CK_DailyCounters_LastNumber", "[LastNumber] >= 0"));
    }
}
