using System.Threading.Tasks;
using SCMS.Data;

namespace SCMS.Interfaces
{
    public interface IPageService
    {
        Task<PageContent?> GetPageBySlugAsync(string slug);
    }
}