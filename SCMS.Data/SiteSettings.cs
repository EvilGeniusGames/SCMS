using System.ComponentModel.DataAnnotations;

namespace SCMS.Data
{
    public class SiteSettings
    {
        public int Id { get; set; }
        public string SiteName { get; set; } = "My Site";
        public string? Tagline { get; set; }
        public string? Logo { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactAddress { get; set; }
        public string? Copyright { get; set; }

        // Social links (1-to-many)
        public ICollection<SiteSocialLink> SocialLinks { get; set; } = new List<SiteSocialLink>();

        [Required]
        public int ThemeId { get; set; }
        public ThemeSetting? Theme { get; set; }

    }

}