using System.ComponentModel.DataAnnotations;
using QueuEase.Data.Enums;

namespace QueuEase.Data.Entities;

public class Ticket
{
    public int Id { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    // The queue the ticket is in now; changes on redirect.
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    // The service the number was issued for; never changes.
    public int IssuedServiceId { get; set; }
    public Service IssuedService { get; set; } = null!;

    // Local date (Europe/Sofia) the number belongs to.
    public DateOnly IssueDate { get; set; }
    public int SequenceNumber { get; set; }
    public string DisplayNumber { get; set; } = null!;

    public TicketStatus Status { get; set; } = TicketStatus.Waiting;
    public TicketSource Source { get; set; }

    public int? CounterId { get; set; }
    public Counter? Counter { get; set; }

    // The employee serving the ticket now. Who issued a staff ticket is recorded in TicketEvent.
    public string? EmployeeId { get; set; }
    public ApplicationUser? Employee { get; set; }

    // All times are UTC.
    // IssuedAt never changes; expiry (TicketExpiryHours) is counted from it.
    public DateTime IssuedAt { get; set; }
    // QueuedAt decides the order in the queue; a "to the end of the queue" redirect resets it.
    public DateTime QueuedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? ServiceStartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public ICollection<TicketEvent> Events { get; set; } = new List<TicketEvent>();
}
