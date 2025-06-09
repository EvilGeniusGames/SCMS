using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace SCMS.Controllers
{
    [Route("media")]
    public class MediaController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public MediaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("secure/{*filename}")]
        //TODO: modify this to secure by Role when rrole managment is complete.
        [Authorize(Roles = "Administrator,User")]
        public IActionResult Secure(string filename)
        {
            var path = Path.Combine(_env.ContentRootPath, "uploads", "protected", filename);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetMimeType(path);
            return PhysicalFile(path, contentType);
        }

        [HttpGet("public/{*filename}")]
        [AllowAnonymous]
        public IActionResult Public(string filename)
        {
            var path = Path.Combine(_env.ContentRootPath, "uploads", "public", filename);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetMimeType(path);
            return PhysicalFile(path, contentType);
        }

        private string GetMimeType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(path, out var type) ? type : "application/octet-stream";
        }
    }
}
