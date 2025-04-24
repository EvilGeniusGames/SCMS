using Microsoft.AspNetCore.Mvc;
using SCMS.Interfaces;
using System.Threading.Tasks;
using SCMS.Services;

namespace SCMS.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [Route("{slug?}")]
        public async Task<IActionResult> RenderPage(string? slug = "home")
        {
            var page = await _pageService.GetPageBySlugAsync(slug ?? "home");
            if (page == null) return NotFound();

            var html = await ThemeEngine.RenderAsync(page);
            return Content(html, "text/html");
        }
    }
}