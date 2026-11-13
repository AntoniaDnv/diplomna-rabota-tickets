using Microsoft.AspNetCore.Mvc;
using QueuEase.Data;

namespace QueuEase.Web.Controllers;

// GET /api/health
// The frontend calls it to show that the whole chain works: browser -> backend -> database.
[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly QueuEaseDbContext db;

    public HealthController(QueuEaseDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var databaseIsUp = await db.Database.CanConnectAsync(cancellationToken);

        // ASP.NET Core turns the object into JSON: { "status": "ok", "database": true }
        return Ok(new { status = "ok", database = databaseIsUp });
    }
}
