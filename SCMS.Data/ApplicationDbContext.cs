using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SCMS.Data.Configuration;
using SCMS.Services; // For ModelBuilderInitializer

namespace SCMS.Data
{
    // DbContext configured for ASP.NET Identity and CMS data
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Initialize Identity customizations and seed default data
            ModelBuilderInitializer.InitAll(modelBuilder);
        }

        // SCMS Entities
        public DbSet<ThemeSetting> ThemeSettings { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<SecurityLevel> SecurityLevels { get; set; }
        public DbSet<SecurityLevelRole> SecurityLevelRoles { get; set; }
    }
}
