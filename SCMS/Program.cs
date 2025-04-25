using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Interfaces;
using SCMS.Services;
using SCMS.Classes;

var builder = WebApplication.CreateBuilder(args);

// Configure database context with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add ASP.NET Identity with default settings
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // You can change this as needed
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Razor Pages (this is the key fix for the error you're seeing)
builder.Services.AddRazorPages();  // Add Razor Pages services

// Add other services
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPageService, PageService>();

var app = builder.Build();

// Seed admin user from environment variables if enabled
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedAdminUserAsync(services); // Ensure this is called once, as needed
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

// Ensure authentication middleware is added here
app.UseAuthentication(); // This is necessary for handling authentication-related logic
app.UseAuthorization();  // Keep this as it is for handling authorization

// Custom CMS Route
app.MapControllerRoute(
    name: "default",
    pattern: "{slug?}",
    defaults: new { controller = "Page", action = "RenderPage" });

app.MapRazorPages();  // Map Razor Pages

// Ensure theme assets are in place
ThemeAssetManager.EnsureThemeAssets();

app.Run();
