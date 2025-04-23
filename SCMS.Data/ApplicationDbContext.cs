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

            // TODO: Add CMS-specific model configurations here
        }

        // TODO: Define your DbSets here (e.g. Pages, MediaFiles, Posts, etc.)
    }
}
