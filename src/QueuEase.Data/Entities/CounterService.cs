namespace QueuEase.Data.Entities;

// Which services a counter serves. Lower Priority is called first in ServicePriority mode.
public class CounterService
{
    public int CounterId { get; set; }
    public Counter Counter { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    // Copied from the counter and the service. Both foreign keys include it,
    // so the database rejects a link between a counter and a service of different locations.
    public int LocationId { get; set; }

    public int Priority { get; set; }
}
