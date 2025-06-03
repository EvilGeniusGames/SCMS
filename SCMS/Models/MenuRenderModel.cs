namespace SCMS.Models
{
    public class MenuRenderModel
    {
        public List<MenuItemModel> Items { get; set; } = new();
    }

    public class MenuItemModel
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public List<MenuItemModel> Children { get; set; } = new();
    }
}
