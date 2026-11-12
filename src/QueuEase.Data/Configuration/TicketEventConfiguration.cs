using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Entities;
using QueuEase.Data.Enums;

namespace QueuEase.Data.Configuration;

public class TicketEventConfiguration : IEntityTypeConfiguration<TicketEvent>
{
    public void Configure(EntityTypeBuilder<TicketEvent> builder)
    {
        builder.HasOne(e => e.Ticket)
            .WithMany(t => t.Events)
            .HasForeignKey(e => e.TicketId);

        // The queue the ticket was in before and after the event (they differ only on a redirect).
        builder.HasOne(e => e.FromService)
            .WithMany()
            .HasForeignKey(e => e.FromServiceId);

        builder.HasOne(e => e.ToService)
            .WithMany()
            .HasForeignKey(e => e.ToServiceId);

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId);

        // A redirect must say where from and where to, and it must change the queue.
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TicketEvents_Redirect",
            $"[Type] <> {(int)TicketEventType.Redirected} OR ([FromServiceId] IS NOT NULL AND [ToServiceId] IS NOT NULL AND [FromServiceId] <> [ToServiceId])"));
    }
}
