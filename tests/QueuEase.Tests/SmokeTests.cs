using QueuEase.Web.Controllers;

namespace QueuEase.Tests;

public class SmokeTests
{
    [Fact]
    public void SolutionBuilds()
    {
        Assert.NotNull(typeof(HealthController).Assembly);
    }
}
