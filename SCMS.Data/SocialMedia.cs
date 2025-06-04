using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS.Data
{
    public class SocialMedia
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // e.g., "Facebook", "Twitter"
        public string Url { get; set; } = "";  // e.g., "https://facebook.com/yourpage"
        public string? IconClass { get; set; } // e.g., "fab fa-facebook-f"
        // Foreign key to SiteSettings
        public int SiteSettingsId { get; set; }
        [ForeignKey(nameof(SiteSettingsId))]
        public SiteSettings SiteSettings { get; set; }
    }
}
