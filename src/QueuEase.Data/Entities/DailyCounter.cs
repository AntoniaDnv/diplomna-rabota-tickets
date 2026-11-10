using System.ComponentModel.DataAnnotations;

namespace QueuEase.Data.Entities;

// Last number issued per location, service and day. Used to give each ticket a unique daily number.
public class DailyCounter
{
    public int Id { get; set; }

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public DateOnly Date { get; set; }
    public int LastNumber { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
