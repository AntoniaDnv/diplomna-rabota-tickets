using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueuEase.Data;
using QueuEase.Data.Entities;
using QueuEase.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

// The real value comes from User Secrets (development) or environment variables (server).
// appsettings.json keeps it empty, so no connection string ever reaches GitHub.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is empty. Set it with: " +
        "dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<value>\" --project src/QueuEase.Web");
}

// true in appsettings.Development.json, false everywhere else.
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

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Brings the database up to the latest migration on every start (creates it the first time).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QueuEaseDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Who are you? (reads the login cookie) must come before What may you do?
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
