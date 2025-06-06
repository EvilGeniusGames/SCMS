using System.Collections.Generic;
using SCMS.Data;

namespace SCMS.Models
{
    public class NavContentViewModel
    {
        public List<MenuGroupView> Groups { get; set; } = new();
    }

    public class MenuGroupView
    {
        public string GroupName { get; set; } = "";
        public List<MenuItem> Items { get; set; } = new();
    }
}
