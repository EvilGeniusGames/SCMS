namespace SCMS.Models
{
    public class MenuItemUpdateModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Url { get; set; }
        public bool IsExternal { get; set; }
        public bool IsVisible { get; set; }
        public int SecurityLevelId { get; set; }
        public string? HtmlContent { get; set; }
    }
}
