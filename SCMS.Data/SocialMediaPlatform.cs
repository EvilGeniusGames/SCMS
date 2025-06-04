using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMS.Data
{
    public class SocialMediaPlatform
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";        // e.g., "Facebook"
        public string IconClass { get; set; } = "";   // e.g., "fab fa-facebook-f"
    }
}

