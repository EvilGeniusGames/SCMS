using Microsoft.AspNetCore.Mvc;
using SCMS.Interfaces;
using SCMS.Services;
using SCMS.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SCMS.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ApplicationDbContext _db;

        public PageController(IPageService pageService, ApplicationDbContext db)
        {
            _pageService = pageService;
            _db = db;
        }

        [Route("{slug?}")]
        public async Task<IActionResult> RenderPage(string? slug = "home")
        {
            Console.WriteLine($"[DEBUG] Requested slug: {slug}");
            var page = await _pageService.GetPageBySlugAsync(slug ?? "home");
            if (page == null) return NotFound();

            var html = await ThemeEngine.RenderAsync(page, _db);
            return Content(html, "text/html");
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePasswordPost(string Input_OldPassword, string Input_NewPassword, string Input_ConfirmPassword)
        {
            if (Input_NewPassword != Input_ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return Redirect("/change-password");
            }

            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/login");
            }

            var result = await userManager.ChangePasswordAsync(user, Input_OldPassword, Input_NewPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await userManager.UpdateAsync(user);
                await signInManager.SignOutAsync();
                return Redirect("/login");
            }

            TempData["Error"] = "Password change failed: " + string.Join(" ", result.Errors.Select(e => e.Description));
            return Redirect("/change-password");
        }

        [HttpGet]
        [Route("portal-logout")]
        public async Task<IActionResult> Logout()
        {
            var signInManager = HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            await signInManager.SignOutAsync();

            // 🔁 Instead of redirecting, fall through to page renderer
            var page = await _pageService.GetPageBySlugAsync("portal-logout");
            if (page == null) return NotFound();

            var html = await ThemeEngine.RenderAsync(page, _db);
            return Content(html, "text/html");
        }



        private void SeedSamplePagesAndMenus(ApplicationDbContext db)
        {
            if (!db.PageContents.Any(p => p.PageKey == "alpha"))
            {
                var pages = new List<PageContent>
        {
            new PageContent
            {
                Id = 1001,
                PageKey = "alpha",
                Title = "Alpha",
                HtmlContent = "<p>Welcome to the Alpha page</p>",
                LastUpdated = DateTime.UtcNow
            },
            new PageContent
            {
                Id = 1002,
                PageKey = "beta",
                Title = "Beta",
                HtmlContent = "<p>Welcome to the Beta page</p>",
                LastUpdated = DateTime.UtcNow
            },
            new PageContent
            {
                Id = 1003,
                PageKey = "gamma",
                Title = "Gamma",
                HtmlContent = "<p>Welcome to the Gamma page</p>",
                LastUpdated = DateTime.UtcNow
            }
        };

                db.PageContents.AddRange(pages);
                db.SaveChanges();

                var menuItems = new List<MenuItem>();
                var subPages = new List<PageContent>();

                int topId = 2001;
                int childId = 3001;
                int pageId = 4001;

                foreach (var page in pages)
                {
                    // Add top level menu item
                    menuItems.Add(new MenuItem
                    {
                        Id = topId,
                        Title = page.Title,
                        PageContentId = page.Id,
                        ParentId = null,
                        MenuGroup = "Main",
                        Order = topId - 2000,
                        IsVisible = true
                    });

                    // Add 3 subpages and submenus for each
                    for (int i = 1; i <= 3; i++)
                    {
                        var subPageKey = $"{page.PageKey}-sub-{i}";
                        var subPageTitle = $"{page.Title} Sub {i}";

                        subPages.Add(new PageContent
                        {
                            Id = pageId,
                            PageKey = subPageKey,
                            Title = subPageTitle,
                            HtmlContent = $"<p>Welcome to the {subPageTitle} page</p>",
                            LastUpdated = DateTime.UtcNow
                        });

                        menuItems.Add(new MenuItem
                        {
                            Id = childId,
                            Title = subPageTitle,
                            PageContentId = pageId,
                            ParentId = topId,
                            MenuGroup = "Main",
                            Order = i,
                            IsVisible = true
                        });

                        childId++;
                        pageId++;
                    }

                    topId++;
                }

                db.PageContents.AddRange(subPages);
                db.MenuItems.AddRange(menuItems);
                db.SaveChanges();
            }
        }

    }
}
