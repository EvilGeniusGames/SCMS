using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Interfaces;
using SCMS.Services;
using SCMS.Classes;
using SCMS.Areas.Identity.Services;
using SCMS.Services.Auth;
using SCMS.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Explicit config override setup for environment support
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure database context with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
Console.WriteLine($"[DEBUG] Connection String: {connectionString}");

// Add ASP.NET Identity with default settings
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/portal-access";
    options.LogoutPath = "/portal-logout";
    options.AccessDeniedPath = "/access-denied"; // optional, if you have one
});

// Custom signin manager
builder.Services.AddScoped<SignInManager<ApplicationUser>, CustomSignInManager>();

// Add Razor Pages and MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddHttpContextAccessor();
// Add the user context service
builder.Services.AddScoped<CurrentUserContext>();

var app = builder.Build();

ThemeEngine.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

app.UseMiddleware<CurrentUserMiddleware>();
// Ensure database folder exists BEFORE context resolution
if (!Directory.Exists("database"))
{
    Directory.CreateDirectory("database");
}

// Seed admin user and apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await IdentitySeeder.SeedAdminUserAsync(services);
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Custom CMS route

app.MapControllerRoute(
    name: "default",
    pattern: "{slug?}",
    defaults: new { controller = "Page", action = "RenderPage" });



// Ensure theme assets are in place
ThemeAssetManager.EnsureThemeAssets();

app.Run();
