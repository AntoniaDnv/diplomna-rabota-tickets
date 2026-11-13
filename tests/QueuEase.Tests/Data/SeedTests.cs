using System.Text.RegularExpressions;
using QueuEase.Data.Common;
using QueuEase.Data.Seeding;

namespace QueuEase.Tests.Data;

public class SeedTests
{
    // The four roles of the system, each with its own fixed Id.
    [Fact]
    public void RoleSeed_HasFourDistinctRoles()
    {
        var roles = RoleSeed.Roles;

        Assert.Equal(new[] { "Admin", "Employee", "Manager", "SuperAdmin" }, roles.Select(r => r.Name).Order());
        Assert.Equal(4, roles.Select(r => r.Id).Distinct().Count());
    }

    [Fact]
    public void DemoLocation_HasTheFourServices()
    {
        var location = DemoDataSeeder.CreateDemoOrganization().Locations.Single();

        Assert.Equal(new[] { "A", "B", "К", "Л" }, location.Services.Select(s => s.Prefix));
    }

    [Fact]
    public void DemoPrefixes_MatchPrefixPattern()
    {
        var services = DemoDataSeeder.CreateDemoOrganization().Locations.Single().Services;

        Assert.NotEmpty(services);
        Assert.All(services, s => Assert.Matches(new Regex(DataConstants.PrefixPattern), s.Prefix));
    }

    [Fact]
    public void DemoCounters_ServeOnlyServicesOfTheirLocation()
    {
        var location = DemoDataSeeder.CreateDemoOrganization().Locations.Single();
        var links = location.Counters.SelectMany(c => c.CounterServices).ToList();

        Assert.Equal(6, links.Count);
        Assert.All(links, link => Assert.Contains(link.Service, location.Services));
    }

    [Fact]
    public void DemoTokens_AreRandomAndValid()
    {
        var first = DemoDataSeeder.CreateDemoOrganization().Locations.Single();
        var second = DemoDataSeeder.CreateDemoOrganization().Locations.Single();

        // Same rules as the CHECK constraints on Locations.
        Assert.Matches($"^[A-Za-z0-9]{{{DataConstants.TokenLength}}}$", first.KioskToken);
        Assert.Matches($"^[A-Za-z0-9]{{{DataConstants.TokenLength}}}$", first.BoardToken);
        Assert.Matches($"^[A-Z0-9]{{{DataConstants.JoinCodeLength}}}$", first.JoinCode);

        Assert.NotEqual(first.KioskToken, first.BoardToken);
        Assert.NotEqual(first.KioskToken, second.KioskToken);
    }
}
