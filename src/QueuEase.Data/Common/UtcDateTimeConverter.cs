using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace QueuEase.Data.Common;

// Used by QueuEaseDbContext.ConfigureConventions for every DateTime and DateTime? property.
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(value => ToDatabase(value), value => FromDatabase(value))
    {
    }

    // A local or "unspecified" time would be saved as if it were UTC and shift by 2-3 hours,
    // so it is a bug in the calling code and fails loudly instead.
    private static DateTime ToDatabase(DateTime value) =>
        value.Kind == DateTimeKind.Utc
            ? value
            : throw new ArgumentException($"Only UTC times can be saved, but got Kind = {value.Kind}.");

    private static DateTime FromDatabase(DateTime value) =>
        DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
