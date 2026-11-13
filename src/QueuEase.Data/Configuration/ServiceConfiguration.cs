using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data.Configuration;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(DataConstants.NameMaxLength);

        builder.Property(s => s.NameEn)
            .HasMaxLength(DataConstants.NameMaxLength);

        // nvarchar, because prefixes can be Cyrillic (К, Л).
        builder.Property(s => s.Prefix)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(DataConstants.PrefixMaxLength);

        // Two queues in the same location cannot share a prefix, or their ticket numbers would clash.
        // Look-alike letters (Latin A and Cyrillic А) are checked by the service layer, not here.
        builder.HasIndex(s => new { s.LocationId, s.Prefix }).IsUnique();

        // (Id, LocationId) is unique because Id alone already is. Tickets, counter links and daily
        // counters point to this pair, so the database guarantees they use a service of their own location.
        builder.HasAlternateKey(s => new { s.Id, s.LocationId });

        builder.HasOne(s => s.Location)
            .WithMany(l => l.Services)
            .HasForeignKey(s => s.LocationId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Services_Name", "LEN([Name]) > 0");
            t.HasCheckConstraint("CK_Services_NameEn", "[NameEn] IS NULL OR LEN([NameEn]) > 0");
            // The same rule as DataConstants.PrefixPattern (^[A-ZА-Я]{1,3}$). SQL Server has no regex,
            // so: 1 to 3 characters (2 bytes each in nvarchar; DATALENGTH, unlike LEN, counts trailing
            // spaces) and no character outside A-Z and А-Я. The binary collation makes the ranges exact.
            t.HasCheckConstraint(
                "CK_Services_Prefix",
                "DATALENGTH([Prefix]) BETWEEN 2 AND 6 AND [Prefix] NOT LIKE N'%[^A-ZА-Я]%' COLLATE Latin1_General_100_BIN2");
        });
    }
}
