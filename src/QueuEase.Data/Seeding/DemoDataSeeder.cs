using Microsoft.EntityFrameworkCore;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;
using QueuEase.Data.Enums;

namespace QueuEase.Data.Seeding;

// A demo ДКЦ registration desk, so the system can be shown right after the first start.
//
// Why not HasData: HasData values are written into the migration, so they would be public on GitHub
// and identical in every database. The kiosk and board tokens are secret keys, so here they are
// random and generated once, when the database is empty.
//
// Program.cs plugs these methods into UseSeeding/UseAsyncSeeding (only when demo data is enabled).
// EF Core calls them after Database.Migrate().
public static class DemoDataSeeder
{
    public const string OrganizationName = "ДКЦ Демо";

    public static void Seed(DbContext context, bool _)
    {
        if (context.Set<Organization>().Any())
        {
            return;
        }

        context.Add(CreateDemoOrganization());
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken cancellationToken)
    {
        // Only an empty database is seeded, so restarting the app never duplicates the demo.
        if (await context.Set<Organization>().AnyAsync(cancellationToken))
        {
            return;
        }

        context.Add(CreateDemoOrganization());
        await context.SaveChangesAsync(cancellationToken);
    }

    // The whole demo is one object graph; EF inserts it in the right order
    // (organization, location, services, counters, links) and fills in the generated Ids.
    public static Organization CreateDemoOrganization()
    {
        var appointment = Service("Записан час", "Appointment", "A");
        var walkIn = Service("Без час", "Walk-in", "B");
        var cashier = Service("Каса", "Cashier", "К");
        var laboratory = Service("Лаборатория", "Laboratory", "Л");

        // Counters 1 and 2 serve the registration queues, counter 3 the cashier and the laboratory.
        var counter1 = Counter("Гише 1", (appointment, 1), (walkIn, 2));
        var counter2 = Counter("Гише 2", (appointment, 1), (walkIn, 2));
        var counter3 = Counter("Гише 3", (cashier, 1), (laboratory, 2));

        var location = new Location
        {
            Name = "Регистратура",
            Address = "гр. София, ул. Примерна 1",
            KioskToken = SecureCodes.NewToken(),
            BoardToken = SecureCodes.NewToken(),
            JoinCode = SecureCodes.NewJoinCode(),
            AutoPrint = false,
            TicketExpiryHours = DataConstants.TicketExpiryHoursDefault,
            Services = { appointment, walkIn, cashier, laboratory },
            Counters = { counter1, counter2, counter3 },
        };

        return new Organization
        {
            Name = OrganizationName,
            Status = OrganizationStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Locations = { location },
        };
    }

    private static Service Service(string name, string nameEn, string prefix) => new()
    {
        Name = name,
        NameEn = nameEn,
        Prefix = prefix,
    };

    private static Counter Counter(string name, params (Service Service, int Priority)[] services)
    {
        var counter = new Counter { Name = name, CallingMode = CallingMode.ArrivalOrder };

        foreach (var (service, priority) in services)
        {
            counter.CounterServices.Add(new CounterService { Service = service, Priority = priority });
        }

        return counter;
    }
}
