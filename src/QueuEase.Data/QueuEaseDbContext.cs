using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;

namespace QueuEase.Data;

public class QueuEaseDbContext : IdentityDbContext<ApplicationUser>
{
    public QueuEaseDbContext(DbContextOptions<QueuEaseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Counter> Counters => Set<Counter>();
    public DbSet<CounterService> CounterServices => Set<CounterService>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketEvent> TicketEvents => Set<TicketEvent>();
    public DbSet<DailyCounter> DailyCounters => Set<DailyCounter>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Every DateTime in the system is UTC. SQL Server's datetime2 does not remember that,
        // so the converter rejects non-UTC values on save and marks values as UTC on load.
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Identity tables first, then one configuration class per entity (the Configuration folder).
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(QueuEaseDbContext).Assembly);

        // Our own entities are the ones in the Entities namespace. An entity placed elsewhere
        // would not get the two rules below.
        var ourEntities = builder.Model.GetEntityTypes()
            .Where(e => e.ClrType.Namespace == typeof(Ticket).Namespace)
            .ToList();

        RestrictDeletes(ourEntities);
        AddEnumCheckConstraints(ourEntities);
    }

    // Nothing is deleted in cascade: tickets and their history must survive, and SQL Server
    // rejects multiple cascade paths (Ticket -> Service and Ticket -> Location -> Service).
    // Identity's own tables (roles, claims, logins) keep their default rules.
    // This overrides any OnDelete written in a configuration class.
    private static void RestrictDeletes(IEnumerable<IMutableEntityType> entities)
    {
        var foreignKeys = entities
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => !fk.IsOwnership
                         && fk.PrincipalEntityType.ClrType.Namespace == typeof(Ticket).Namespace);

        foreach (var foreignKey in foreignKeys)
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    // An enum is stored as int, and SQL Server would accept any int (Status = 99).
    // This adds, for example, CK_Tickets_Status: [Status] IN (0, 1, 2, 3, 4, 5).
    // Adding a value to an enum later changes the constraint, so it shows up in a new migration.
    private static void AddEnumCheckConstraints(IEnumerable<IMutableEntityType> entities)
    {
        foreach (var entity in entities)
        {
            foreach (var property in entity.GetProperties())
            {
                var type = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
                if (!type.IsEnum)
                {
                    continue;
                }

                var column = property.GetColumnName();
                var values = string.Join(", ", Enum.GetValues(type).Cast<object>().Select(Convert.ToInt32));

                entity.AddCheckConstraint($"CK_{entity.GetTableName()}_{column}", $"[{column}] IN ({values})");
            }
        }
    }
}
