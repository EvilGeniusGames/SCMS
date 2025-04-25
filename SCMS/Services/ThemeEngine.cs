using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SCMS.Classes; // 👈 new import for MenuBuilder

namespace SCMS.Services
{
    public static class ThemeEngine
    {
        public static IHttpContextAccessor HttpContextAccessor { get; set; }

        public static async Task<string> RenderAsync(PageContent page, ApplicationDbContext db)
        {
            var siteSettings = await db.SiteSettings.FirstOrDefaultAsync();
            var themeName = siteSettings?.Theme?.Name ?? "default";
            var themePath = Path.Combine("Themes", themeName);

            var layout = await File.ReadAllTextAsync(Path.Combine(themePath, "layout.html"));
            var template = await File.ReadAllTextAsync(Path.Combine(themePath, "templates", "page.html"));
            var header = await File.ReadAllTextAsync(Path.Combine(themePath, "partials", "header.html"));
            var footer = await File.ReadAllTextAsync(Path.Combine(themePath, "partials", "footer.html"));

            var faviconPath = Path.Combine("/Themes", themeName, siteSettings?.Theme?.Favicon ?? "favicon.ico");
            var logoUrl = siteSettings?.Logo ?? "/Themes/default/images/SCMS_Logo.png";

            var copyright =
                string.IsNullOrWhiteSpace(siteSettings?.Copyright)
                ? $"© {DateTime.Now.Year} {siteSettings?.SiteName ?? "SCMS"}"
                : siteSettings.Copyright;

            var tagline = siteSettings?.Tagline ?? "Site Powered by SCMS";

            var body = template
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", page.HtmlContent ?? "");

            var result = layout
                .Replace("<cms:Header />", header)
                .Replace("<cms:Footer />", footer)
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", body)
                .Replace("<cms:Favicon />", $"<link rel=\"icon\" href=\"{faviconPath}\" type=\"image/x-icon\">")
                .Replace("<cms:Copyright />", copyright)
                .Replace("<cms:Tagline />", tagline);

            // Handle dynamic login/logout button
            var user = HttpContextAccessor?.HttpContext?.User;
            bool isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
            string loginStatusHtml = isAuthenticated
                ? "<div class=\"login-status\"><a href=\"/Account/Logout\">Logout</a></div>"
                : "<div class=\"login-status\"><a href=\"/Account/Login\">Login</a></div>";
            result = result.Replace("<cms:LoginStatus />", loginStatusHtml);

            // Replace menu token with generated menu HTML
            var menuRegex = new Regex(
                @"<cms:Menu\s+(?=.*orientation=""(?<orientation>\w+)""\s*)(?=.*group=""(?<group>[^""]+)""\s*).*?\/?>",
                RegexOptions.IgnoreCase
            );

            result = menuRegex.Replace(result, match =>
            {
                var orientation = match.Groups["orientation"].Value;
                var group = match.Groups["group"].Value;
                return MenuBuilder.GenerateMenuHtml(db, group, orientation);
            });

            // Replace <cms:SiteLogo height="..."/> with an image tag
            var logoRegex = new Regex(@"<cms:SiteLogo(?:\s+height\s*=\s*""(?<height>\d+)"")?\s*\/>");
            result = logoRegex.Replace(result, match =>
            {
                var height = match.Groups["height"].Success ? match.Groups["height"].Value : "50";
                return $"<img src=\"{logoUrl}\" alt=\"Site Logo\" style=\"max-height: {height}px;\">";
            });

            // Highlight unknown tokens
            result = Regex.Replace(result, @"<cms:[^>]+\/>", match =>
            {
                var safeToken = match.Value.Replace("<", "(").Replace(">", ")");
                return $"<span style='color: red; font-weight: bold;'>[UNKNOWN TOKEN: {safeToken}]</span>";
            });

            return result;
        }
    }
}
