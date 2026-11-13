using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueuEase.Data;
using QueuEase.Data.Entities;
using QueuEase.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Where the value comes from:
//   - dotnet run on this machine: User Secrets
//   - Docker: the environment variable ConnectionStrings__DefaultConnection (docker-compose.yml)
// appsettings.json keeps it empty, so no connection string ever reaches GitHub.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is empty. Set it with: " +
        "dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<value>\" --project src/QueuEase.Web");
}

// true in appsettings.Development.json and in docker-compose.yml, false everywhere else.
var seedDemoData = builder.Configuration.GetValue<bool>("Seeding:DemoData");

builder.Services.AddDbContext<QueuEaseDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    if (seedDemoData)
    {
        // EF Core calls these after Migrate(); the seeder itself skips a non-empty database.
        options.UseSeeding(DemoDataSeeder.Seed);
        options.UseAsyncSeeding(DemoDataSeeder.SeedAsync);
    }
});

// Users and roles are stored in the same database through QueuEaseDbContext.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<QueuEaseDbContext>()
    .AddDefaultTokenProviders();

// The backend has no pages of its own any more, so it cannot redirect to a login page.
// It answers with a status code and the React app decides what to show.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized; // not logged in
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden; // logged in, but not allowed
        return Task.CompletedTask;
    };
});

// Controllers only (JSON in, JSON out). The pages are in the React app (the frontend folder).
builder.Services.AddControllers();

// Errors are returned as JSON in the standard "problem details" format.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Brings the database up to the latest migration on every start (creates it the first time).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QueuEaseDbContext>();
    await db.Database.MigrateAsync();
}

// An unhandled exception becomes a JSON 500 answer instead of an HTML error page.
app.UseExceptionHandler();

// No UseHttpsRedirection here: the backend is reached only through the frontend's nginx,
// inside the Docker network. HTTPS is handled in front of nginx by the hosting.

// Who are you? (reads the login cookie) must come before What may you do?
app.UseAuthentication();
app.UseAuthorization();

// Routes come from the [Route] attributes on the controllers, for example /api/health.
app.MapControllers();

await app.RunAsync();
