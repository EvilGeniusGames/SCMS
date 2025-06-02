
using Microsoft.AspNetCore.Identity;

namespace SCMS.Data
{
	public class ApplicationUser : IdentityUser
    {
		public bool MustChangePassword { get; set; } = false;
	}
}
