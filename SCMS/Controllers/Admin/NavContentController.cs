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

            var model = new NavContentViewModel
            {
                Groups = menuItems
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
                viewData: new Dictionary<string, object>()
            );

            var wrapped = await ThemeEngine.RenderAsync(new PageContent
            {
                Title = "Menu/Page Editor",
                HtmlContent = html
            }, _context);

            return Content(wrapped, "text/html");
        }
    }
}
