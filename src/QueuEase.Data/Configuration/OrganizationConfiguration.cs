using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(DataConstants.NameMaxLength);

        // NOT NULL alone still allows "" and "   "; LEN ignores trailing spaces, so both are rejected.
        builder.ToTable(t => t.HasCheckConstraint("CK_Organizations_Name", "LEN([Name]) > 0"));
    }
}
