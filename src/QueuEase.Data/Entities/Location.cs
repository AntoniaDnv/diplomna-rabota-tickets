using QueuEase.Data.Common;

namespace QueuEase.Data.Entities;

public class Location
{
    public int Id { get; set; }

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    // Secret part of the kiosk URL; regenerating it disables the old link.
    public string KioskToken { get; set; } = null!;
    public string BoardToken { get; set; } = null!;
    // Given by the manager to new employees so they can register for this location.
    public string JoinCode { get; set; } = null!;

    public bool AutoPrint { get; set; }
    public int TicketExpiryHours { get; set; } = DataConstants.TicketExpiryHoursDefault;
    public bool IsActive { get; set; } = true;

    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Counter> Counters { get; set; } = new List<Counter>();
    public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
