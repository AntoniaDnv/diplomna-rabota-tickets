using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    // Binary collation compares exact characters: "abc" and "ABC" are different,
    // and LIKE ranges such as [A-Z] mean exactly those code points.
    private const string BinaryCollation = "Latin1_General_100_BIN2";

    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(DataConstants.NameMaxLength);

        builder.Property(l => l.Address)
            .IsRequired()
            .HasMaxLength(DataConstants.AddressMaxLength);

        // Tokens and the join code use only Latin letters and digits, so varchar is enough.
        // Tokens are case-sensitive (binary collation), otherwise "aB3" and "Ab3" would be the same key.
        builder.Property(l => l.KioskToken)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(DataConstants.TokenMaxLength)
            .UseCollation(BinaryCollation);

        builder.Property(l => l.BoardToken)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(DataConstants.TokenMaxLength)
            .UseCollation(BinaryCollation);

        // The join code keeps the default case-insensitive collation, because people type it.
        builder.Property(l => l.JoinCode)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(DataConstants.JoinCodeLength);

        // Each token must point to exactly one location.
        builder.HasIndex(l => l.KioskToken).IsUnique();
        builder.HasIndex(l => l.BoardToken).IsUnique();
        builder.HasIndex(l => l.JoinCode).IsUnique();

        builder.HasOne(l => l.Organization)
            .WithMany(o => o.Locations)
            .HasForeignKey(l => l.OrganizationId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Locations_Name", "LEN([Name]) > 0");
            t.HasCheckConstraint("CK_Locations_Address", "LEN([Address]) > 0");
            t.HasCheckConstraint(
                "CK_Locations_TicketExpiryHours",
                $"[TicketExpiryHours] BETWEEN {DataConstants.TicketExpiryHoursMin} AND {DataConstants.TicketExpiryHoursMax}");
            // Generated tokens are long random strings; a short one would be easy to guess.
            t.HasCheckConstraint(
                "CK_Locations_KioskToken",
                $"LEN([KioskToken]) >= {DataConstants.TokenLength} AND [KioskToken] NOT LIKE '%[^A-Za-z0-9]%' COLLATE {BinaryCollation}");
            t.HasCheckConstraint(
                "CK_Locations_BoardToken",
                $"LEN([BoardToken]) >= {DataConstants.TokenLength} AND [BoardToken] NOT LIKE '%[^A-Za-z0-9]%' COLLATE {BinaryCollation}");
            // Exactly 8 capital Latin letters or digits.
            t.HasCheckConstraint(
                "CK_Locations_JoinCode",
                $"LEN([JoinCode]) = {DataConstants.JoinCodeLength} AND [JoinCode] NOT LIKE '%[^A-Z0-9]%' COLLATE {BinaryCollation}");
        });
    }
}
