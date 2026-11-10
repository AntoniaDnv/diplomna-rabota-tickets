namespace QueuEase.Data.Entities;

// One service is one queue, with its own prefix and daily numbering.
public class Service
{
    public int Id { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? NameEn { get; set; }
    public string Prefix { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public ICollection<CounterService> CounterServices { get; set; } = new List<CounterService>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
