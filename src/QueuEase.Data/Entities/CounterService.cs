namespace QueuEase.Data.Entities;

// Which services a counter serves. Lower Priority is called first in ServicePriority mode.
public class CounterService
{
    public int CounterId { get; set; }
    public Counter Counter { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int Priority { get; set; }
}
