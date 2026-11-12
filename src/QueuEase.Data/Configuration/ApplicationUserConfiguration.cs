using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(DataConstants.NameMaxLength);

        builder.HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId);

        builder.HasOne(u => u.Location)
            .WithMany(l => l.Employees)
            .HasForeignKey(u => u.LocationId);

        // Identity creates a normal index on NormalizedEmail; this makes it unique,
        // so two accounts cannot share an e-mail even if the code forgets to check.
        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasFilter("[NormalizedEmail] IS NOT NULL");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_AspNetUsers_FullName", "LEN([FullName]) > 0");
            // A user with a location always belongs to an organization.
            // The role rules (SuperAdmin has none, Admin has no location...) are checked in the service layer.
            t.HasCheckConstraint("CK_AspNetUsers_LocationNeedsOrganization", "[LocationId] IS NULL OR [OrganizationId] IS NOT NULL");
        });
    }
}
