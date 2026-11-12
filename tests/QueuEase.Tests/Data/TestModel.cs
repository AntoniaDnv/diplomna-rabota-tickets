using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using QueuEase.Data;

namespace QueuEase.Tests.Data;

// The EF Core model of QueuEaseDbContext, built once for all tests.
// UseSqlServer only chooses the SQL Server rules; no connection is opened.
public static class TestModel
{
    public static IModel Instance { get; } = Create();

    public static IEntityType Entity<T>() => Instance.FindEntityType(typeof(T))!;

    private static IModel Create()
    {
        var options = new DbContextOptionsBuilder<QueuEaseDbContext>()
            .UseSqlServer("Server=.;Database=ModelOnly")
            .Options;

        // The design-time model keeps everything migrations need, such as index filters and CHECKs.
        using var context = new QueuEaseDbContext(options);
        return context.GetService<IDesignTimeModel>().Model;
    }
}
