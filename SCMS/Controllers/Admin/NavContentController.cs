using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Models;
using SCMS.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCMS.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    [Route("admin/[controller]")]
    public class NavContentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RazorRenderer _razorRenderer;

        public NavContentController(ApplicationDbContext context, RazorRenderer razorRenderer)
        {
            _context = context;
            _razorRenderer = razorRenderer;
        }

        [HttpGet("/admin/navcontent")]
        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.PageContent)
                .Include(m => m.SecurityLevel)
                .ToListAsync();

            // Find the Admin root node
            var adminRoot = menuItems.FirstOrDefault(m => m.Title == "Admin");

            // Gather all admin node IDs recursively
            var adminIds = new HashSet<int>();
            if (adminRoot != null)
            {
                void CollectDescendants(int parentId)
                {
                    var children = menuItems.Where(m => m.ParentId == parentId).ToList();
                    foreach (var child in children)
                    {
                        if (adminIds.Add(child.Id))
                            CollectDescendants(child.Id);
                    }
                }

                adminIds.Add(adminRoot.Id);
                CollectDescendants(adminRoot.Id);
            }

            // Filter out Admin node and descendants
            var filteredItems = menuItems
                .Where(m => !adminIds.Contains(m.Id))
                .ToList();

            var model = new NavContentViewModel
            {
                Groups = filteredItems
                    .GroupBy(m => m.MenuGroup)
                    .Select(g => new MenuGroupView
                    {
                        GroupName = g.Key,
                        Items = g.OrderBy(m => m.Order).ToList()
                    })
                    .ToList()
            };

            var html = await _razorRenderer.RenderViewAsync<NavContentViewModel>(
                HttpContext,
                "/Views/Admin/NavContent/Index.cshtml",
                model,
                new Dictionary<string, object>()
            );

            var wrapped = await ThemeEngine.RenderAsync(new PageContent
            {
                Title = "Menu/Page Editor",
                HtmlContent = html
            }, _context);

            var result = wrapped;
            var isAdminPage = HttpContext.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase);

            // Font Awesome
            if (isAdminPage && !result.Contains("font-awesome/6.5.1/css/all.min.css"))
            {
                result = result.Replace("</head>", @"
<link href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css"" rel=""stylesheet"">
</head>");
            }

            // Bootstrap
            if (isAdminPage && !result.Contains("bootstrap@5.3.2/dist/css/bootstrap.min.css"))
            {
                result = result.Replace("</head>", @"
<link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"" 
      rel=""stylesheet"" 
      integrity=""sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN"" 
      crossorigin=""anonymous"">
</head>");
            }

            // Quill CSS
            if (isAdminPage && !result.Contains("quill@2.0.3/dist/quill.snow.css"))
            {
                result = result.Replace("</head>", @"
<link href=""https://cdn.jsdelivr.net/npm/quill@2.0.3/dist/quill.snow.css"" rel=""stylesheet"">
</head>");
            }

            // Quill JS
            if (isAdminPage && !result.Contains("quill@2.0.3/dist/quill.js"))
            {
                result = result.Replace("</body>", @"
<script src=""https://cdn.jsdelivr.net/npm/quill@2.0.3/dist/quill.js""></script>
</body>");
            }

            return Content(result, "text/html");
        }
    }
}
