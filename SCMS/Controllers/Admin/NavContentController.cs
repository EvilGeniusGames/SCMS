﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Models;
using SCMS.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCMS.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    [Route("admin/[controller]")]
    public class NavContentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RazorRenderer _razorRenderer;
        private readonly IWebHostEnvironment _env;
        public NavContentController(ApplicationDbContext context, RazorRenderer razorRenderer, IWebHostEnvironment env)
        {
            _context = context;
            _razorRenderer = razorRenderer;
            _env = env;
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

            //tinymce 
            if (!result.Contains("/lib/tinymce/tinymce.min.js"))
            {
                result = result.Replace("</body>", @"
                <script src=""/lib/tinymce/tinymce.min.js""></script>
                </body>");
            }

            // TinyMCE Fullscreen css
            if (isAdminPage && !result.Contains("/css/tiny-full.css"))
            {
                result = result.Replace("</head>", @"
                <link href=""/css/tiny-full.css"" rel=""stylesheet"">
                </head>");
            }


            return Content(result, "text/html");
        }

        [HttpGet("load/{id}")]
        public async Task<IActionResult> Load(int id)
        {
            var item = await _context.MenuItems
                .Include(m => m.PageContent)
                .Include(m => m.SecurityLevel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            return Json(new
            {
                item.Id,
                item.Title,
                item.Url,
                item.IsVisible,
                item.SecurityLevelId,
                pageTitle = item.PageContent?.Title,
                item.PageContent?.HtmlContent,
                item.PageContent?.MetaDescription,
                MetaKeywords = item.PageContent?.MetaKeywords?.Split(',').Select(k => k.Trim()).ToList()
            });
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] MenuItemUpdateModel model)
        {
            var item = await _context.MenuItems
                .Include(m => m.PageContent)
                .FirstOrDefaultAsync(m => m.Id == model.Id);

            if (item == null)
                return NotFound();

            item.Title = model.Title;
            item.Url = model.IsExternal ? model.Url : null;
            item.IsVisible = model.IsVisible;

            // Check for security level change
            bool securityChanged = item.PageContent != null && item.SecurityLevelId != model.SecurityLevelId;
            item.SecurityLevelId = model.SecurityLevelId;

            var htmlContent = model.HtmlContent ?? "";

            if (!model.IsExternal)
            {
                var matches = Regex.Matches(
                    htmlContent,
                    @"<img[^>]*?src=['""](?<src>(?:\.\./)*(media/secure|uploads/(temp|public|protected))/[^'""]+)['""]",
                    RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    var rawSrc = match.Groups["src"].Value;
                    var currentSrc = rawSrc.TrimStart('.', '/');
                    var fileName = Path.GetFileName(currentSrc);

                    string oldPath;
                    if (currentSrc.StartsWith("media/secure", StringComparison.OrdinalIgnoreCase))
                    {
                        oldPath = Path.Combine(_env.ContentRootPath, "uploads", "protected", fileName);
                    }
                    else if (currentSrc.StartsWith("uploads/public", StringComparison.OrdinalIgnoreCase))
                    {
                        oldPath = Path.Combine(_env.WebRootPath, "uploads", "public", fileName);
                    }
                    else // includes temp or fallback
                    {
                        oldPath = Path.Combine(_env.WebRootPath, currentSrc.Replace('/', Path.DirectorySeparatorChar));
                    }

                    string newPath, newSrc;
                    if (model.SecurityLevelId == 3)
                    {
                        newPath = Path.Combine(_env.WebRootPath, "uploads", "public", fileName);
                        newSrc = $"/uploads/public/{fileName}";
                    }
                    else
                    {
                        newPath = Path.Combine(_env.ContentRootPath, "uploads", "protected", fileName);
                        newSrc = $"/media/secure/{fileName}";
                    }

                    if (System.IO.File.Exists(oldPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                        System.IO.File.Move(oldPath, newPath, overwrite: true);
                        htmlContent = htmlContent.Replace(rawSrc, newSrc);
                    }
                }

                if (item.PageContent == null)
                {
                    item.PageContent = new PageContent
                    {
                        Title = model.PageTitle ?? item.Title,
                        HtmlContent = htmlContent,
                        MetaDescription = model.MetaDescription,
                        MetaKeywords = string.Join(", ", model.MetaKeywords ?? new List<string>())
                    };
                }
                else
                {
                    item.PageContent.Title = model.PageTitle ?? item.Title;
                    item.PageContent.HtmlContent = htmlContent;
                    item.PageContent.MetaDescription = model.MetaDescription;
                    item.PageContent.MetaKeywords = string.Join(", ", model.MetaKeywords ?? new List<string>());
                }
            }
            else
            {
                if (item.PageContent != null)
                    _context.PageContents.Remove(item.PageContent);

                item.PageContent = null;
                item.PageContentId = null;
            }

            await ReindexSiblingsAsync(item.MenuGroup, item.ParentId);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("group/add")]
        public async Task<IActionResult> AddGroup([FromBody] GroupNameModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Group name required.");

            var page = new PageContent
            {
                Title = "New Page",
                HtmlContent = "<p>New page content.</p>",
                PageKey = Slugify(model.Name) + "-root"
            };


            var newItem = new MenuItem
            {
                Title = "New Menu Item",
                MenuGroup = model.Name,
                Order = 0,
                IsVisible = true,
                SecurityLevelId = 3,
                PageContent = page
            };

            _context.PageContents.Add(page);
            _context.MenuItems.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("group/rename")]
        public async Task<IActionResult> RenameGroup([FromBody] GroupRenameModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewName) || string.IsNullOrWhiteSpace(model.OldName))
                return BadRequest("Both old and new group names are required.");

            var items = await _context.MenuItems
                .Where(m => m.MenuGroup == model.OldName)
                .ToListAsync();

            if (!items.Any()) return NotFound("Group not found.");

            foreach (var item in items)
                item.MenuGroup = model.NewName;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("group/delete")]
        public async Task<IActionResult> DeleteGroup([FromBody] GroupNameModel model)
        {
            if(string.IsNullOrWhiteSpace(model.Name))
            return BadRequest("Group name required.");

            if (model.Name.Equals("Main", StringComparison.OrdinalIgnoreCase))
                return BadRequest("The 'Main' group cannot be deleted.");

            var items = await _context.MenuItems
                .Where(m => m.MenuGroup == model.Name)
                .ToListAsync();

            var pageIds = items
            .Where(i => i.PageContentId.HasValue)
            .Select(i => i.PageContentId.Value)
            .ToList();

            var pages = await _context.PageContents
                .Where(p => pageIds.Contains(p.Id))
                .ToListAsync();

            _context.PageContents.RemoveRange(pages);


            if (!items.Any()) return NotFound("Group not found.");

            _context.MenuItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("group/items/{groupName}")]
        public async Task<IActionResult> LoadGroupItems(string groupName)
        {
            var items = await _context.MenuItems
                .Where(m => m.MenuGroup == groupName && m.Title != "Admin")
                .OrderBy(m => m.Order)
                .ToListAsync();

            return PartialView("_MenuTreePartial", items);
        }

        [HttpPost("item/create")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateItemModel model)
        {
            var siblings = await _context.MenuItems
                .Where(m => m.MenuGroup == model.Group && m.ParentId == model.ParentId)
                .OrderBy(m => m.Order)
                .ToListAsync();

            int insertIndex = siblings.FindIndex(i => i.Id == model.InsertAfterId);
            if (insertIndex == -1) insertIndex = siblings.Count;

            // Shift all items after insertIndex forward
            for (int i = insertIndex + 1; i < siblings.Count; i++)
            {
                siblings[i].Order = i + 1;
            }

            // Create new item at insert position
            var newItem = new MenuItem
            {
                Title = model.Title,
                MenuGroup = model.Group,
                ParentId = model.ParentId,
                Order = insertIndex + 1,
                IsVisible = true,
                SecurityLevelId = 3,
                PageContent = new PageContent
                {
                    Title = model.Title,
                    HtmlContent = "<p>New content</p>",
                    PageKey = Guid.NewGuid().ToString("N")
                }
            };

            _context.PageContents.Add(newItem.PageContent);
            _context.MenuItems.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("item/delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.MenuItems
                .Include(m => m.PageContent)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            if (item.PageContent != null)
                _context.PageContents.Remove(item.PageContent);

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("group/structure/{groupName}")]
        public async Task<IActionResult> GetGroupStructure(string groupName)
        {
            var items = await _context.MenuItems
                .Where(m => m.MenuGroup == groupName)
                .OrderBy(m => m.Order)
                .Select(m => new {
                    m.Id,
                    m.Title,
                    m.Order,
                    m.ParentId
                })
                .ToListAsync();

            List<object> flattened = new();
            void AddWithChildren(int? parentId)
            {
                var children = items.Where(i => i.ParentId == parentId).OrderBy(i => i.Order).ToList();
                foreach (var child in children)
                {
                    flattened.Add(child);
                    AddWithChildren(child.Id);
                }
            }

            AddWithChildren(null);
            return Json(flattened);
        }

        [HttpPost("item/set-parent")]
        public async Task<IActionResult> SetParent([FromBody] ParentUpdateModel model)
        {
            var item = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == model.Id);
            if (item == null) return NotFound();

            item.ParentId = model.ParentId;
            await ReindexSiblingsAsync(item.MenuGroup, item.ParentId);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("item/reorder")]
        public async Task<IActionResult> Reorder([FromBody] List<ReorderItem> items)
        {
            foreach (var item in items)
            {
                var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == item.Id);
                if (menuItem != null)
                {
                    if (menuItem.Id == 1 && menuItem.Title == "Admin")
                    {
                        menuItem.Order = 9999; // Always force Admin to end
                    }
                    else
                    {
                        menuItem.Order = item.Order;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // reindexer for menu items
        private async Task ReindexSiblingsAsync(string menuGroup, int? parentId)
        {
            var siblings = await _context.MenuItems
                .Where(m => m.MenuGroup == menuGroup && m.ParentId == parentId && m.Id != 1)
                .OrderBy(m => m.Order)
                .ToListAsync();

            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].Order = i;
            }

            // Explicitly reset Admin order if it exists in this scope
            var admin = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == 1);
            if (admin != null)
            {
                admin.Order = 9999;
            }

            await _context.SaveChangesAsync();
        }


        // make pagekey from group name
        private static string Slugify(string input)
        {
            return input
                .ToLowerInvariant()
                .Trim()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("&", "")
                .Replace("#", "")
                .Replace("--", "-");
        }

        // DTOs
        public class MenuItemUpdateModel
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string? Url { get; set; }
            public bool IsExternal { get; set; }
            public bool IsVisible { get; set; }
            public int SecurityLevelId { get; set; }
            public string? HtmlContent { get; set; }
            public string? PageTitle { get; set; }
            public string? MetaDescription { get; set; }
            public List<string>? MetaKeywords { get; set; }
        }
        public class GroupNameModel
        {
            public string Name { get; set; } = "";
        }
        public class GroupRenameModel
        {
            public string OldName { get; set; } = "";
            public string NewName { get; set; } = "";
        }
        public class CreateItemModel
        {
            public string Title { get; set; } = "";
            public string Group { get; set; } = "";
            public int? ParentId { get; set; }
            public int? InsertAfterId { get; set; }
        }
        public class ParentUpdateModel
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
        }
        public class ReorderItem
        {
            public int Id { get; set; }
            public int Order { get; set; }
        }

    }
}
