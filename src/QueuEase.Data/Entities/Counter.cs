using QueuEase.Data.Enums;

namespace QueuEase.Data.Entities;

public class Counter
{
    public int Id { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public string Name { get; set; } = null!;
    public CallingMode CallingMode { get; set; } = CallingMode.ArrivalOrder;

    // The employee currently working at this counter; null when the counter is closed.
    public string? CurrentEmployeeId { get; set; }
    public ApplicationUser? CurrentEmployee { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<CounterService> CounterServices { get; set; } = new List<CounterService>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
