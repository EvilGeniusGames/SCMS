using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Interfaces;
using SCMS.Services;

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
builder.Services.AddScoped<IPageService, PageService>();
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

// 🧩 Custom CMS Route
app.MapControllerRoute(
    name: "default",
    pattern: "{slug?}",
    defaults: new { controller = "Page", action = "RenderPage" });

EnsureThemeAssets();

app.Run();

void EnsureThemeAssets()
{
    var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Themes", "default");
    var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Themes", "default");

    var assetFolders = new[] { "css", "js", "images", "fonts" };

    foreach (var folder in assetFolders)
    {
        var srcDir = Path.Combine(sourcePath, folder);
        var tgtDir = Path.Combine(targetPath, folder);

        if (!Directory.Exists(tgtDir))
            Directory.CreateDirectory(tgtDir);

        if (Directory.Exists(srcDir))
        {
            foreach (var file in Directory.GetFiles(srcDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(tgtDir, fileName);

                if (!File.Exists(destFile))
                {
                    File.Copy(file, destFile);
                }
            }
        }
    }
}
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
