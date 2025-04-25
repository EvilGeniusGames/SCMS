using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS.Data
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int? ParentId { get; set; } // For nested menus
        public string Title { get; set; } = string.Empty;
        public string? Url { get; set; } // Optional external URL
        public int? PageContentId { get; set; } // Optional linked PageContent
        public string MenuGroup { get; set; } = "Main"; // e.g. Main, Footer, Sidebar
        public int Order { get; set; } = 0;
        public bool IsVisible { get; set; } = true;

        [ForeignKey(nameof(PageContentId))]
        public PageContent? PageContent { get; set; }
    }
}
