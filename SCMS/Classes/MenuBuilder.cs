using SCMS.Data;
using System.Linq;
using System.Text;
using SCMS.Models;
using SCMS.Services.Theme;
using SCMS.Services.Template;
using System.Security.Claims;

namespace SCMS.Classes
{
    // Responsible for generating HTML for menus based on MenuItems in the database
    public static class MenuBuilder
    {
        // Generates the complete HTML for a menu group with a given orientation (horizontal or vertical)
        public static string GenerateMenuHtml(ApplicationDbContext db, string group, string orientation, ClaimsPrincipal user)
        {
            var allItems = db.MenuItems
                .Where(m => m.MenuGroup == group && m.IsVisible)
                .OrderBy(m => m.Order)
                .ToList();

            if (!allItems.Any()) return "";

            var topLevelItems = allItems
            .Where(m => m.ParentId == null && IsMenuItemAuthorized(db, m, user))
            .ToList();


            // Convert to MenuRenderModel
            var model = new MenuRenderModel
            {
                Items = topLevelItems.Select(t => ConvertToModel(t, allItems, db, user)).ToList()
            };

            var context = new Dictionary<string, object>
            {
                { "Items", model.Items },
                { "Orientation", orientation }
            };

            // Locate theme and template
            var theme = ThemeManager.GetCurrentTheme(); // Replace with actual theme getter if needed
            var templatePath = Path.Combine("Themes", theme, "partials", "menu.template.html");

            if (System.IO.File.Exists(templatePath))
            {
                var template = System.IO.File.ReadAllText(templatePath);
                var parser = new TemplateParser();
                return parser.Parse(template, context);
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
        private static MenuItemModel ConvertToModel(MenuItem item, List<MenuItem> allItems, ApplicationDbContext db, ClaimsPrincipal user)
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
                .Where(m => m.ParentId == item.Id && IsMenuItemAuthorized(db, m, user))
                .OrderBy(m => m.Order)
                .Select(child => ConvertToModel(child, allItems, db, user))
                .ToList();

            return new MenuItemModel
            {
                Text = item.Title,
                Url = url,
                Target = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? "_blank" : null,
                Items = children
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
        // TODO: When building the admin group manager, replace this logic with role-to-user resolution
        // that supports dynamic group membership and custom role assignments.
        private static bool IsMenuItemAuthorized(ApplicationDbContext db, MenuItem item, ClaimsPrincipal user)
        {
            if (item.SecurityLevelId == 3)
                return true;

            if (!user.Identity?.IsAuthenticated ?? true)
                return false;

            // Get all roles assigned to this user
            var roleNames = db.SecurityLevelRoles
                .Select(r => r.RoleName)
                .Distinct()
                .ToList();

            var userRoles = roleNames
                .Where(user.IsInRole)
                .ToList();

            if (!userRoles.Any()) return false;

            // Get minimum security level ID for user’s roles
            var userMinLevel = db.SecurityLevelRoles
                .Where(r => userRoles.Contains(r.RoleName))
                .Min(r => r.SecurityLevelId);

            return userMinLevel <= item.SecurityLevelId;
        }
        public static string GenerateBreadcrumbHtml(ApplicationDbContext db, string currentUrl, ClaimsPrincipal user)
        {
            var allItems = db.MenuItems
                .Where(m => m.IsVisible)
                .ToList();

            var currentItem = allItems
                .FirstOrDefault(mi => string.Equals(mi.Url?.Trim('/'), currentUrl.Trim('/'), StringComparison.OrdinalIgnoreCase));

            if (currentItem == null) return "";

            var breadcrumb = new Stack<MenuItem>();
            var walker = currentItem;

            while (walker != null)
            {
                breadcrumb.Push(walker);
                walker = walker.ParentId.HasValue ? allItems.FirstOrDefault(m => m.Id == walker.ParentId.Value) : null;
            }

            var sb = new StringBuilder();
            sb.Append("<nav aria-label=\"breadcrumb\"><ol class=\"breadcrumb mb-0\">");

            while (breadcrumb.Count > 0)
            {
                var item = breadcrumb.Pop();
                var title = item.Title;
                var url = item.Url?.Trim();

                if (breadcrumb.Count > 0 && !string.IsNullOrWhiteSpace(url) && url != "#")
                    sb.Append($"<li class=\"breadcrumb-item\"><a href=\"{url}\">{title}</a></li>");
                else
                    sb.Append($"<li class=\"breadcrumb-item active\" aria-current=\"page\">{title}</li>");

            }

            sb.Append("</ol></nav>");
            return sb.ToString();
        }

    }
}
