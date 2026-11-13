using QueuEase.Data.Enums;

namespace QueuEase.Data.Entities;

// History of a ticket. Rows are only added, never changed.
public class TicketEvent
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public TicketEventType Type { get; set; }

    public int? FromServiceId { get; set; }
    public Service? FromService { get; set; }

    public int? ToServiceId { get; set; }
    public Service? ToService { get; set; }

    public string? EmployeeId { get; set; }
    public ApplicationUser? Employee { get; set; }

    public DateTime OccurredAt { get; set; }
}
