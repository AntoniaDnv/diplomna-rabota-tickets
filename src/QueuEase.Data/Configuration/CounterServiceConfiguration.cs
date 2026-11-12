using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class CounterServiceConfiguration : IEntityTypeConfiguration<CounterService>
{
    public void Configure(EntityTypeBuilder<CounterService> builder)
    {
        // A counter can be linked to a given service only once.
        builder.HasKey(cs => new { cs.CounterId, cs.ServiceId });

        // Both foreign keys share LocationId, so the counter and the service must be in the same location.
        builder.HasOne(cs => cs.Counter)
            .WithMany(c => c.CounterServices)
            .HasForeignKey(cs => new { cs.CounterId, cs.LocationId })
            .HasPrincipalKey(c => new { c.Id, c.LocationId });

        builder.HasOne(cs => cs.Service)
            .WithMany(s => s.CounterServices)
            .HasForeignKey(cs => new { cs.ServiceId, cs.LocationId })
            .HasPrincipalKey(s => new { s.Id, s.LocationId });

        // 1 is called first; 0 or negative values have no meaning.
        builder.ToTable(t => t.HasCheckConstraint("CK_CounterServices_Priority", "[Priority] > 0"));
    }
}
