using Microsoft.AspNetCore.Identity;
using SCMS.Data;

namespace SCMS.Services.Auth
{
    public class CurrentUserContext
    {
        public ApplicationUser? User { get; private set; }
        public bool IsAuthenticated => User != null;

        public async Task InitializeAsync(HttpContext httpContext, UserManager<ApplicationUser> userManager)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated == true && userManager != null)
            {
                User = await userManager.GetUserAsync(httpContext.User);
            }
        }
    }
}
