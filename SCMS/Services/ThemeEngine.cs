using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SCMS.Classes;
using Microsoft.AspNetCore.Antiforgery;
using System.Security.Claims;

namespace SCMS.Services
{
    public static class ThemeEngine
    {
        public static IHttpContextAccessor HttpContextAccessor { get; set; }

        public static async Task<string> RenderAsync(PageContent page, ApplicationDbContext db)
        {
            var siteSettings = await db.SiteSettings
            .Include(s => s.SocialLinks)
            .ThenInclude(l => l.Platform)
            .FirstOrDefaultAsync();

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

            var bodyContent = page.HtmlContent ?? "";

            // Inject TempData["Error"] if it exists
            var tempData = HttpContextAccessor?.HttpContext?.RequestServices
                .GetService<ITempDataDictionaryFactory>()
                ?.GetTempData(HttpContextAccessor.HttpContext);

            var body = template
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", bodyContent);

            var result = layout
                .Replace("<cms:Header />", header)
                .Replace("<cms:Footer />", footer)
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", body)
                .Replace("<cms:Favicon />", $"<link rel=\"icon\" href=\"{faviconPath}\" type=\"image/x-icon\">")
                .Replace("<cms:Copyright />", copyright)
                .Replace("<cms:Tagline />", tagline);

            var user = HttpContextAccessor?.HttpContext?.User;
            bool isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
            string loginStatusHtml = isAuthenticated
                ? "<a href=\"/portal-logout\">Logout</a>"
                : "<a href=\"/portal-access\">Login</a>";
            result = result.Replace("<cms:LoginStatus />", loginStatusHtml);

            // Replace <cms:ErrorMessage /> with alert if TempData["Error"] exists
            if (result.Contains("<cms:ErrorMessage />"))
            {
                var err = tempData?["Error"] as string;
                var errorHtml = !string.IsNullOrEmpty(err)
                    ? $"<div class='alert alert-danger'>{err}</div>"
                    : "";
                result = result.Replace("<cms:ErrorMessage />", errorHtml);
            }

            // Handle <cms:Menu />
            var menuRegex = new Regex(
                @"<cms:Menu\s+(?=.*orientation=""(?<orientation>\w+)""\s*)(?=.*group=""(?<group>[^""]+)""\s*).*?\/?>",
                RegexOptions.IgnoreCase
            );

            result = menuRegex.Replace(result, match =>
            {
                var orientation = match.Groups["orientation"].Value;
                var group = match.Groups["group"].Value;
                var principal = HttpContextAccessor?.HttpContext?.User ?? new ClaimsPrincipal();
                return MenuBuilder.GenerateMenuHtml(db, group, orientation, principal);
            });

            // Handle <cms:SiteLogo height="..." />
            var logoRegex = new Regex(@"<cms:SiteLogo(?:\s+height\s*=\s*""(?<height>\d+)"")?\s*\/>");
            result = logoRegex.Replace(result, match =>
            {
                var height = match.Groups["height"].Success ? match.Groups["height"].Value : "50";
                return $"<img src=\"{logoUrl}\" alt=\"Site Logo\" style=\"max-height: {height}px;\">";
            });

            var displayName = user?.Identity?.IsAuthenticated == true
            ? (HttpContextAccessor?.HttpContext?.User.Identity?.Name ?? "User")
            : "Guest";
            result = result.Replace("<cms:UserName />", displayName);

            // Handle {{ANTIFORGERY_TOKEN}} replacement
            if (result.Contains("{{ANTIFORGERY_TOKEN}}"))
            {
                var antiforgery = HttpContextAccessor?.HttpContext?.RequestServices.GetService<IAntiforgery>();
                var tokenSet = antiforgery?.GetAndStoreTokens(HttpContextAccessor.HttpContext);
                var tokenValue = tokenSet?.RequestToken ?? "";
                result = result.Replace("{{ANTIFORGERY_TOKEN}}", tokenValue);
            }

            // Handle <cms:SocialLinks />
            var socialLinksRegex = new Regex(@"<cms:SocialLinks\s*\/>", RegexOptions.IgnoreCase);

            if (socialLinksRegex.IsMatch(result))
            {
                var templatePath = Path.Combine(themePath, "partials", "social.template.html");
                string renderedSocial = "";

                if (File.Exists(templatePath))
                {
                    var templateText = await File.ReadAllTextAsync(templatePath);

                    var links = siteSettings?.SocialLinks?.Where(l => l.Platform != null).Select(link =>
                        new Dictionary<string, object>
                        {
                            ["Url"] = link.Url,
                            ["Name"] = link.Platform.Name,
                            ["IconClass"] = link.Platform.IconClass,
                            ["IconColor"] = link.IconColor ?? "#000000"
                        }
                    ).ToList<object>() ?? new List<object>();

                    var socialData = new Dictionary<string, object>
                    {
                        ["Items"] = links
                    };

                    var parser = new SCMS.Services.Template.TemplateParser();
                    renderedSocial = parser.Parse(templateText, socialData);
                }

                result = socialLinksRegex.Replace(result, renderedSocial);
            }

            // Catch unknown tokens and replace with UNKNOWN Leave at bottom
            result = Regex.Replace(result, @"<cms:[^>]+\/>", match =>
            {
                var safeToken = match.Value.Replace("<", "(").Replace(">", ")");
                return $"<span style='color: red; font-weight: bold;'>[UNKNOWN TOKEN: {safeToken}]</span>";
            });

            //embed font awesome becuase its a requirment for the operation of the site,
            const string fontAwesomeCdn = "<link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css\" rel=\"stylesheet\">";
            if (!result.Contains("cdnjs.cloudflare.com/ajax/libs/font-awesome"))
            {
                result = result.Replace("</head>", $"{fontAwesomeCdn}\n</head>");
            }

            // Return the final rendered HTML
            return result;
        }
    }
}
