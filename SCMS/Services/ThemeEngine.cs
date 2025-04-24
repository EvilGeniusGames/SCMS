using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SCMS.Data;

namespace SCMS.Services
{
    public static class ThemeEngine
    {
        public static async Task<string> RenderAsync(PageContent page)
        {
            var themePath = Path.Combine("Themes", "default");
            var layout = await File.ReadAllTextAsync(Path.Combine(themePath, "layout.html"));
            var template = await File.ReadAllTextAsync(Path.Combine(themePath, "templates", "page.html"));
            var header = await File.ReadAllTextAsync(Path.Combine(themePath, "partials", "header.html"));
            var footer = await File.ReadAllTextAsync(Path.Combine(themePath, "partials", "footer.html"));

            var body = template
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", page.HtmlContent ?? "");

            var result = layout
                .Replace("<cms:Header />", header)
                .Replace("<cms:Footer />", footer)
                .Replace("<cms:PageTitle />", page.Title ?? "")
                .Replace("<cms:Content />", body);

            result = Regex.Replace(result, @"<cms:[^>]+\/>", match =>
            {
                var safeToken = match.Value.Replace("<", "(").Replace(">", ")");
                return $"[UNKNOWN TOKEN: {safeToken}]";
            });
            return result;
        }
    }
}
