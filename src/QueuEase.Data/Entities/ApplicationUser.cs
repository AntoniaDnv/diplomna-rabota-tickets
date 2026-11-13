using Microsoft.AspNetCore.Identity;

namespace QueuEase.Data.Entities;

// SuperAdmin: no organization. Admin: organization only. Manager and Employee: organization and location.
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = null!;

    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public bool IsApproved { get; set; }
    public bool MustChangePassword { get; set; }
}
