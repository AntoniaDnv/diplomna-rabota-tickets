using QueuEase.Data.Enums;

namespace QueuEase.Data.Entities;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public OrganizationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
