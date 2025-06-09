using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace SCMS.Controllers.Admin
{
    [Authorize(Roles = "Administrator")]
    [Route("admin/upload")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("❌ Upload failed: no file received.");
                return BadRequest("No file uploaded.");
            }

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", "temp");
            Directory.CreateDirectory(uploadsPath);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            Console.WriteLine($"📥 Uploading file: {fileName} to {filePath}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"✅ Upload complete: /uploads/temp/{fileName}");

            return Json(new { location = $"/uploads/temp/{fileName}" });

        }
    }
}
