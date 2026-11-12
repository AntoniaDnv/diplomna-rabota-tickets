using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QueuEase.Data.Common;
using QueuEase.Data.Entities;
using QueuEase.Data.Enums;
using static QueuEase.Tests.Data.TestModel;

namespace QueuEase.Tests.Data;

// These tests read the EF Core model only; no database connection is opened.
public class ModelConfigurationTests
{
    private static IIndex? FindIndex<T>(params string[] properties) =>
        Entity<T>().GetIndexes()
            .SingleOrDefault(i => i.Properties.Select(p => p.Name).SequenceEqual(properties));

    private static string? CheckSql<T>(string name) =>
        Entity<T>().GetCheckConstraints().SingleOrDefault(c => c.Name == name)?.Sql;

    // --- Unique indexes ---

    [Fact]
    public void Ticket_HasUniqueDailyNumberIndex()
    {
        var index = FindIndex<Ticket>(
            nameof(Ticket.LocationId), nameof(Ticket.IssuedServiceId),
            nameof(Ticket.IssueDate), nameof(Ticket.SequenceNumber));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void DailyCounter_HasUniqueIndex()
    {
        var index = FindIndex<DailyCounter>(
            nameof(DailyCounter.LocationId), nameof(DailyCounter.ServiceId), nameof(DailyCounter.Date));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Ticket_ActiveCounterIndex_IsFilteredOnCalledAndInService()
    {
        var index = FindIndex<Ticket>(nameof(Ticket.CounterId));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
        Assert.Equal(
            $"[CounterId] IS NOT NULL AND [Status] IN ({(int)TicketStatus.Called}, {(int)TicketStatus.InService})",
            index.GetFilter());
    }

    [Fact]
    public void Counter_CurrentEmployee_IsUniqueWhenSet()
    {
        var index = FindIndex<Counter>(nameof(Counter.CurrentEmployeeId));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
        Assert.Equal("[CurrentEmployeeId] IS NOT NULL", index.GetFilter());
    }

    [Fact]
    public void Service_PrefixIsUniquePerLocation()
    {
        var index = FindIndex<Service>(nameof(Service.LocationId), nameof(Service.Prefix));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Theory]
    [InlineData(nameof(Location.KioskToken))]
    [InlineData(nameof(Location.BoardToken))]
    [InlineData(nameof(Location.JoinCode))]
    public void Location_SecretsAreUnique(string property)
    {
        var index = FindIndex<Location>(property);

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void User_EmailIsUnique()
    {
        var index = FindIndex<ApplicationUser>(nameof(ApplicationUser.NormalizedEmail));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    // --- Keys and relationships ---

    [Fact]
    public void CounterService_HasCompositeKey()
    {
        var key = Entity<CounterService>().FindPrimaryKey()!;

        Assert.Equal(
            new[] { nameof(CounterService.CounterId), nameof(CounterService.ServiceId) },
            key.Properties.Select(p => p.Name));
    }

    // Each of these foreign keys includes LocationId, so the related row must be in the same location.
    [Theory]
    [InlineData(typeof(Ticket), nameof(Ticket.ServiceId))]
    [InlineData(typeof(Ticket), nameof(Ticket.IssuedServiceId))]
    [InlineData(typeof(DailyCounter), nameof(DailyCounter.ServiceId))]
    [InlineData(typeof(CounterService), nameof(CounterService.ServiceId))]
    [InlineData(typeof(CounterService), nameof(CounterService.CounterId))]
    public void SameLocationForeignKeys_IncludeLocationId(Type entity, string foreignKeyProperty)
    {
        var foreignKey = Instance.FindEntityType(entity)!.GetForeignKeys()
            .Single(fk => fk.Properties[0].Name == foreignKeyProperty);

        Assert.Equal(
            new[] { foreignKeyProperty, "LocationId" },
            foreignKey.Properties.Select(p => p.Name));
        Assert.Equal(
            new[] { "Id", "LocationId" },
            foreignKey.PrincipalKey.Properties.Select(p => p.Name));
    }

    [Fact]
    public void AllForeignKeys_ToOurEntities_AreRestrict()
    {
        // Both ends must be ours. This includes AspNetUsers -> Organizations/Locations, but not Identity's
        // own tables (AspNetUserRoles, AspNetUserClaims...) -> AspNetUsers: deleting a user should delete
        // their logins and role links, so those keep Cascade.
        var ourNamespace = typeof(Ticket).Namespace;
        var foreignKeys = Instance.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => fk.DeclaringEntityType.ClrType.Namespace == ourNamespace
                         && fk.PrincipalEntityType.ClrType.Namespace == ourNamespace)
            .ToList();

        Assert.NotEmpty(foreignKeys);
        Assert.All(foreignKeys, fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
    }

    // --- Column types ---

    [Theory]
    [InlineData(typeof(Ticket))]
    [InlineData(typeof(DailyCounter))]
    public void RowVersions_AreConcurrencyTokens(Type entity)
    {
        var rowVersion = Instance.FindEntityType(entity)!.FindProperty("RowVersion")!;

        Assert.True(rowVersion.IsConcurrencyToken);
        Assert.Equal(ValueGenerated.OnAddOrUpdate, rowVersion.ValueGenerated);
    }

    [Fact]
    public void ServicePrefix_IsUnicodeWithMaxLength3()
    {
        var prefix = Entity<Service>().FindProperty(nameof(Service.Prefix))!;

        Assert.True(prefix.IsUnicode() ?? true);
        Assert.Equal(DataConstants.PrefixMaxLength, prefix.GetMaxLength());
    }

    [Theory]
    [InlineData(nameof(Location.KioskToken))]
    [InlineData(nameof(Location.BoardToken))]
    [InlineData(nameof(Location.JoinCode))]
    public void LocationSecrets_AreNotUnicode(string property)
    {
        Assert.False(Entity<Location>().FindProperty(property)!.IsUnicode());
    }

    [Theory]
    [InlineData(nameof(Location.KioskToken))]
    [InlineData(nameof(Location.BoardToken))]
    public void LocationTokens_AreCaseSensitive(string property)
    {
        Assert.Equal("Latin1_General_100_BIN2", Entity<Location>().FindProperty(property)!.GetCollation());
    }

    [Fact]
    public void EveryDateTime_UsesTheUtcConverter()
    {
        var dateTimes = Instance.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
            .ToList();

        Assert.NotEmpty(dateTimes);
        Assert.All(dateTimes, p => Assert.IsType<UtcDateTimeConverter>(p.GetValueConverter()));
    }

    // --- CHECK constraints ---

    [Fact]
    public void TicketExpiryHours_IsLimitedToItsRange()
    {
        Assert.Equal(
            $"[TicketExpiryHours] BETWEEN {DataConstants.TicketExpiryHoursMin} AND {DataConstants.TicketExpiryHoursMax}",
            CheckSql<Location>("CK_Locations_TicketExpiryHours"));
    }

    [Fact]
    public void ServicePrefix_HasPatternCheck()
    {
        Assert.NotNull(CheckSql<Service>("CK_Services_Prefix"));
    }

    [Theory]
    [InlineData(typeof(Organization), "CK_Organizations_Status", typeof(OrganizationStatus))]
    [InlineData(typeof(Counter), "CK_Counters_CallingMode", typeof(CallingMode))]
    [InlineData(typeof(Ticket), "CK_Tickets_Status", typeof(TicketStatus))]
    [InlineData(typeof(Ticket), "CK_Tickets_Source", typeof(TicketSource))]
    [InlineData(typeof(TicketEvent), "CK_TicketEvents_Type", typeof(TicketEventType))]
    public void EnumColumns_AcceptOnlyDefinedValues(Type entity, string constraint, Type enumType)
    {
        var values = string.Join(", ", Enum.GetValues(enumType).Cast<object>().Select(Convert.ToInt32));
        var column = constraint.Split('_')[2];

        var sql = Instance.FindEntityType(entity)!.GetCheckConstraints()
            .Single(c => c.Name == constraint).Sql;

        Assert.Equal($"[{column}] IN ({values})", sql);
    }

    [Theory]
    [InlineData(typeof(Ticket), "CK_Tickets_SequenceNumber")]
    [InlineData(typeof(Ticket), "CK_Tickets_WaitingIsFree")]
    [InlineData(typeof(Ticket), "CK_Tickets_ActiveHasCounter")]
    [InlineData(typeof(Ticket), "CK_Tickets_CompletedHasTimes")]
    [InlineData(typeof(DailyCounter), "CK_DailyCounters_LastNumber")]
    [InlineData(typeof(CounterService), "CK_CounterServices_Priority")]
    [InlineData(typeof(Location), "CK_Locations_JoinCode")]
    [InlineData(typeof(Counter), "CK_Counters_InactiveHasNoEmployee")]
    [InlineData(typeof(TicketEvent), "CK_TicketEvents_Redirect")]
    [InlineData(typeof(ApplicationUser), "CK_AspNetUsers_LocationNeedsOrganization")]
    public void BusinessRules_HaveCheckConstraints(Type entity, string constraint)
    {
        Assert.Contains(
            Instance.FindEntityType(entity)!.GetCheckConstraints(),
            c => c.Name == constraint);
    }
}
