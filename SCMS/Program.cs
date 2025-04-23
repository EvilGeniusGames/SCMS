using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure database context with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add ASP.NET Identity with default settings
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register MVC and Razor Pages
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed admin user from environment variables if enabled
await SeedAdminUserAsync(app);

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
app.UseAuthorization();

// Set up endpoint routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


// Seeds an admin user based on environment variables
static async Task SeedAdminUserAsync(WebApplication app)
{
    var config = app.Configuration;
    if (config["EnableAdminSeeding"] == "true")
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var email = config["AdminEmail"];
        var password = config["AdminPassword"];

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            var adminUser = await userManager.FindByEmailAsync(email);
            if (adminUser == null)
            {
                var user = new IdentityUser { UserName = email, Email = email };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
