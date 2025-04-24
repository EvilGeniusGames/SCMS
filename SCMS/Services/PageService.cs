using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Interfaces;
using System.Threading.Tasks;

namespace SCMS.Services
{
    public class PageService : IPageService
    {
        private readonly ApplicationDbContext _db;

        public PageService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PageContent?> GetPageBySlugAsync(string slug)
        {
            return await _db.PageContents
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PageKey == slug);
        }
    }
}
