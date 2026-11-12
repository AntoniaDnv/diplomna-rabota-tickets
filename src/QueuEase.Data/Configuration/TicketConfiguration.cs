using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;
using QueuEase.Data.Enums;

namespace QueuEase.Data.Configuration;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    private const int Waiting = (int)TicketStatus.Waiting;
    private const int Called = (int)TicketStatus.Called;
    private const int InService = (int)TicketStatus.InService;
    private const int Completed = (int)TicketStatus.Completed;
    private const int NoShow = (int)TicketStatus.NoShow;

    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.DisplayNumber)
            .IsRequired()
            .HasMaxLength(DataConstants.DisplayNumberMaxLength);

        builder.HasOne(t => t.Location)
            .WithMany(l => l.Tickets)
            .HasForeignKey(t => t.LocationId);

        // A ticket points to Service twice, so both relationships are configured explicitly.
        // Both use (ServiceId, LocationId): the queue must belong to the ticket's own location,
        // so a redirect to another location's service is rejected by the database.
        builder.HasOne(t => t.Service)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => new { t.ServiceId, t.LocationId })
            .HasPrincipalKey(s => new { s.Id, s.LocationId });

        builder.HasOne(t => t.IssuedService)
            .WithMany()
            .HasForeignKey(t => new { t.IssuedServiceId, t.LocationId })
            .HasPrincipalKey(s => new { s.Id, s.LocationId });

        // Kept as a simple key: CounterId is optional, and the service layer checks that
        // the counter is in the ticket's location.
        builder.HasOne(t => t.Counter)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CounterId);

        builder.HasOne(t => t.Employee)
            .WithMany()
            .HasForeignKey(t => t.EmployeeId);

        // The same number cannot be issued twice on one day, even if two kiosks are pressed at once.
        builder.HasIndex(t => new { t.LocationId, t.IssuedServiceId, t.IssueDate, t.SequenceNumber })
            .IsUnique();

        // A counter can have only one active ticket (called or in service) at a time.
        builder.HasIndex(t => t.CounterId)
            .IsUnique()
            .HasFilter($"[CounterId] IS NOT NULL AND [Status] IN ({Called}, {InService})");

        // Speeds up the most common query: the waiting tickets of a queue, oldest first.
        builder.HasIndex(t => new { t.ServiceId, t.Status, t.QueuedAt });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Tickets_SequenceNumber", "[SequenceNumber] > 0");
            t.HasCheckConstraint("CK_Tickets_DisplayNumber", "LEN([DisplayNumber]) > 0");

            // The status must agree with the other columns (spec, section 6: the state machine).
            // A waiting ticket is not at a counter and nobody is serving it.
            t.HasCheckConstraint(
                "CK_Tickets_WaitingIsFree",
                $"[Status] <> {Waiting} OR ([CounterId] IS NULL AND [EmployeeId] IS NULL)");
            // A called or served ticket has a counter, an employee and a call time.
            t.HasCheckConstraint(
                "CK_Tickets_ActiveHasCounter",
                $"[Status] NOT IN ({Called}, {InService}) OR ([CounterId] IS NOT NULL AND [EmployeeId] IS NOT NULL AND [CalledAt] IS NOT NULL)");
            t.HasCheckConstraint(
                "CK_Tickets_InServiceHasStart",
                $"[Status] <> {InService} OR [ServiceStartedAt] IS NOT NULL");
            t.HasCheckConstraint(
                "CK_Tickets_CompletedHasTimes",
                $"[Status] <> {Completed} OR ([ServiceStartedAt] IS NOT NULL AND [CompletedAt] IS NOT NULL)");
            t.HasCheckConstraint(
                "CK_Tickets_NoShowWasCalled",
                $"[Status] <> {NoShow} OR [CalledAt] IS NOT NULL");
            t.HasCheckConstraint(
                "CK_Tickets_CompletedAfterStart",
                "[CompletedAt] IS NULL OR [CompletedAt] >= [ServiceStartedAt]");
            t.HasCheckConstraint(
                "CK_Tickets_QueuedAfterIssued",
                "[QueuedAt] >= [IssuedAt]");
        });
    }
}
