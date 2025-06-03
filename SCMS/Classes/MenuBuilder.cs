using SCMS.Data;
using System.Linq;
using System.Text;
using SCMS.Models;
using SCMS.Services.Theme;
using SCMS.Services.Template;

namespace SCMS.Classes
{
    // Responsible for generating HTML for menus based on MenuItems in the database
    public static class MenuBuilder
    {
        // Generates the complete HTML for a menu group with a given orientation (horizontal or vertical)
        public static string GenerateMenuHtml(ApplicationDbContext db, string group, string orientation)
        {
            var allItems = db.MenuItems
                .Where(m => m.MenuGroup == group && m.IsVisible)
                .OrderBy(m => m.Order)
                .ToList();

            if (!allItems.Any()) return "";

            var topLevelItems = allItems.Where(m => m.ParentId == null).ToList();

            // Convert to MenuRenderModel
            var model = new MenuRenderModel
            {
                Items = topLevelItems.Select(t => ConvertToModel(t, allItems, db)).ToList()
            };

            // Locate theme and template
            var theme = ThemeManager.GetCurrentTheme(); // Replace with actual theme getter if needed
            var templatePath = Path.Combine("Themes", theme, "partials", "menu.template.html");

            if (System.IO.File.Exists(templatePath))
            {
                var template = System.IO.File.ReadAllText(templatePath);
                var parser = new TemplateParser();
                return parser.Parse(template, model);
            }
            else
            {
                // fallback to old HTML builder
                var cssClass = orientation == "vertical" ? "nav-menu vertical" : "nav-menu horizontal";
                StringBuilder html = new StringBuilder();
                html.Append($"<ul class=\"{cssClass}\">");
                foreach (var item in topLevelItems)
                {
                    html.Append(BuildMenuItem(db, item, allItems));
                }
                html.Append("</ul>");
                return html.ToString();
            }
        }
        // Converts a MenuItem to a MenuItemModel for rendering
        private static MenuItemModel ConvertToModel(MenuItem item, List<MenuItem> allItems, ApplicationDbContext db)
        {
            string url = "#";

            if (!string.IsNullOrEmpty(item.Url))
                url = item.Url;
            else if (item.PageContentId.HasValue)
            {
                var page = db.PageContents.FirstOrDefault(p => p.Id == item.PageContentId.Value);
                if (page != null) url = "/" + page.PageKey;
            }

            var children = allItems
                .Where(m => m.ParentId.HasValue && m.ParentId.Value == item.Id)
                .OrderBy(m => m.Order)
                .Select(child => ConvertToModel(child, allItems, db))
                .ToList();

            return new MenuItemModel
            {
                Text = item.Title,
                Url = url,
                Children = children
            };
        }


        // Recursively builds HTML for a single menu item, including any nested children
        private static string BuildMenuItem(ApplicationDbContext db, MenuItem item, List<MenuItem> allItems)
        {
            StringBuilder html = new StringBuilder();
            string url = "#";

            // Resolve the URL: prefer the explicit Url, else link to the related PageContent if available
            if (!string.IsNullOrEmpty(item.Url))
                url = item.Url;
            else if (item.PageContentId.HasValue)
            {
                var page = db.PageContents.FirstOrDefault(p => p.Id == item.PageContentId.Value);
                if (page != null) url = "/" + page.PageKey;
            }

            // Start list item
            html.Append($"<li><a href=\"{url}\">{item.Title}</a>");

            // Recursively build any children (submenus)
            var children = allItems.Where(m => m.ParentId == item.Id).OrderBy(m => m.Order).ToList();
            if (children.Any())
            {
                html.Append("<ul class=\"submenu\">");
                foreach (var child in children)
                {
                    html.Append(BuildMenuItem(db, child, allItems));
                }
                html.Append("</ul>");
            }

            // End list item
            html.Append("</li>");
            return html.ToString();
        }
    }
}
