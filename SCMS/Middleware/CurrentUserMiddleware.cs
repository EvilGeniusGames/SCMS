using Microsoft.AspNetCore.Identity;
using SCMS.Data;
using SCMS.Services.Auth;

namespace SCMS.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, CurrentUserContext userContext, UserManager<ApplicationUser> userManager)
        {
            await userContext.InitializeAsync(context, userManager);
            await _next(context);
        }
    }
}