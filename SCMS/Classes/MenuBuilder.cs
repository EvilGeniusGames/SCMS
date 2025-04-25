using SCMS.Data;
using System.Linq;
using System.Text;

namespace SCMS.Classes
{
    // Responsible for generating HTML for menus based on MenuItems in the database
    public static class MenuBuilder
    {
        // Generates the complete HTML for a menu group with a given orientation (horizontal or vertical)
        public static string GenerateMenuHtml(ApplicationDbContext db, string group, string orientation)
        {
            // Load all menu items for the specified group
            var allItems = db.MenuItems
                .Where(m => m.MenuGroup == group && m.IsVisible)
                .OrderBy(m => m.Order)
                .ToList();

            if (!allItems.Any()) return "";

            // Get only top-level menu items (those without a parent)
            var topLevelItems = allItems.Where(m => m.ParentId == null).ToList();
            var cssClass = orientation == "vertical" ? "nav-menu vertical" : "nav-menu horizontal";

            StringBuilder html = new StringBuilder();
            html.Append($"<ul class=\"{cssClass}\">");

            // Build each top-level menu item and any of its children
            foreach (var item in topLevelItems)
            {
                html.Append(BuildMenuItem(db, item, allItems));
            }

            html.Append("</ul>");
            return html.ToString();
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
