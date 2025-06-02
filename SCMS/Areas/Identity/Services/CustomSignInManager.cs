using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SCMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace SCMS.Areas.Identity.Services
{
    public class CustomSignInManager : SignInManager<ApplicationUser>
    {
        public CustomSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override async Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null)
        {
            await base.SignInAsync(user, isPersistent, authenticationMethod);

            // Could hook logic here if you needed to signal redirect.
            // Redirect must still be handled from the login page code-behind.
        }
    }
}
