using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SCMS.Data.Configuration;

namespace SCMS.Data
{
    // DbContext configured for ASP.NET Identity and CMS data
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Configure Identity tables and seed roles
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default roles (Admin, Editor, User)
            modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());

            // Customize schema for IdentityRole to support SQLite
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles");
                entity.Property(r => r.Id).HasColumnType("TEXT");
                entity.Property(r => r.Name).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(r => r.NormalizedName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(r => r.ConcurrencyStamp).HasColumnType("TEXT");
            });

            // Customize schema for IdentityUser to support SQLite
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.Property(u => u.Id).HasColumnType("TEXT");
                entity.Property(u => u.UserName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(u => u.NormalizedUserName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(u => u.Email).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(u => u.NormalizedEmail).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(u => u.ConcurrencyStamp).HasColumnType("TEXT");
                entity.Property(u => u.SecurityStamp).HasColumnType("TEXT");
            });

            modelBuilder.Entity<ThemeSetting>().HasData(new ThemeSetting
            {
                Id = 1,
                Name = "default",
                DisplayName = "Default Theme",
                Description = "Clean layout with light styling",
                PreviewImage = "/Themes/Default/preview.png",
                SetOn = DateTime.UtcNow
            });

            modelBuilder.Entity<SiteSettings>().HasData(new SiteSettings
            {
                Id = 1,
                SiteName = "SCMS Site",
                Tagline = "Powered by SCMS",
                ThemeId = 1
            });

            modelBuilder.Entity<SocialMedia>().HasData(
                new SocialMedia { Id = 1, Name = "Facebook", Url = "#", IconClass = "fab fa-facebook-f" },
                new SocialMedia { Id = 2, Name = "Twitter", Url = "#", IconClass = "fab fa-twitter" },
                new SocialMedia { Id = 3, Name = "Instagram", Url = "#", IconClass = "fab fa-instagram" },
                new SocialMedia { Id = 4, Name = "YouTube", Url = "#", IconClass = "fab fa-youtube" },
                new SocialMedia { Id = 5, Name = "LinkedIn", Url = "#", IconClass = "fab fa-linkedin-in" },
                new SocialMedia { Id = 6, Name = "TikTok", Url = "#", IconClass = "fab fa-tiktok" },
                new SocialMedia { Id = 7, Name = "Pinterest", Url = "#", IconClass = "fab fa-pinterest-p" },
                new SocialMedia { Id = 8, Name = "Bluesky", Url = "#", IconClass = "fas fa-globe" } // Generic globe icon
            );

            modelBuilder.Entity<PageContent>().HasData(new PageContent
            {
                Id = 1,
                PageKey = "home",
                Title = "Welcome",
                HtmlContent = "<p>This is your first SCMS page. Edit it in the admin panel.</p>",
                LastUpdated = DateTime.UtcNow
            });
        }

        public DbSet<ThemeSetting> ThemeSettings { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
    }
}
