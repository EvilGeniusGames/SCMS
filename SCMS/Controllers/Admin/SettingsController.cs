using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using SCMS.Data;
using SCMS.Services;
using System.Linq;
using System.Threading.Tasks;


namespace SCMS.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    [Route("admin/[controller]")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RazorRenderer _razorRenderer;

        public SettingsController(ApplicationDbContext context, RazorRenderer razorRenderer)
        {
            _context = context;
            _razorRenderer = razorRenderer;
        }

        [HttpGet("/admin/settings")]
        public async Task<IActionResult> Settings()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync() ?? new SiteSettings();

            var themes = await _context.ThemeSettings.ToListAsync();
            ViewData["Themes"] = themes.Select(t => new
            {
                t.Id,
                DisplayName = t.DisplayName ?? t.Name,
                IsSelected = settings.ThemeId == t.Id
            }).ToList();

            var html = await _razorRenderer.RenderViewAsync(
                HttpContext,
                "/Views/Admin/Settings/Index.cshtml",
                settings,
                new Dictionary<string, object>
                {
                    ["Themes"] = themes.Select(t => new {
                        t.Id,
                        DisplayName = t.DisplayName ?? t.Name,
                        IsSelected = settings.ThemeId == t.Id
                    }).ToList<dynamic>()
                }
            );

            var wrapped = await ThemeEngine.RenderAsync(new PageContent
            {
                Title = "Site Settings",
                HtmlContent = html
            }, _context);

            return Content(wrapped, "text/html");
        }

        [HttpPost("save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromForm] SiteSettings siteSettings)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid settings: " + string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));

            var current = await _context.SiteSettings.FirstOrDefaultAsync();
            if (current == null)
            {
                _context.SiteSettings.Add(siteSettings);
            }
            else
            {
                current.SiteName = siteSettings.SiteName;
                current.Tagline = siteSettings.Tagline;
                current.ThemeId = siteSettings.ThemeId;
                current.ContactEmail = siteSettings.ContactEmail;
                current.ContactPhone = siteSettings.ContactPhone;
                current.ContactAddress = siteSettings.ContactAddress;
                current.Copyright = siteSettings.Copyright;
                current.SocialLinks = siteSettings.SocialLinks;
                _context.SiteSettings.Update(current);
            }

            await _context.SaveChangesAsync();
            return Redirect("/admin/settings");
        }



    }
}
