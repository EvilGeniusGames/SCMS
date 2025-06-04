using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS.Data;
using SCMS.Data.Configuration;

namespace SCMS.Services
{
    public static class ModelBuilderInitializer
    {
        public static void InitAll(ModelBuilder modelBuilder)
        {
            InitIdentity(modelBuilder);
            SeedDefaultData(modelBuilder);
            SeedIdentityPages(modelBuilder);
        }

        private static void InitIdentity(ModelBuilder modelBuilder)
        {
            // Apply default Identity role configuration
            modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());

            // Customize IdentityRole table for SQLite
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles");
                entity.Property(r => r.Id).HasColumnType("TEXT");
                entity.Property(r => r.Name).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(r => r.NormalizedName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(r => r.ConcurrencyStamp).HasColumnType("TEXT");
            });

            // Customize ApplicationUser table for SQLite
            modelBuilder.Entity<ApplicationUser>(entity =>
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
        }

        private static void SeedIdentityPages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PageContent>().HasData(
            // Login page content
            new PageContent
            {
                Id = 2,
                PageKey = "portal-access",
                Title = "Login",
                HtmlContent = @"
                    <div class=""d-flex align-items-center justify-content-center"">
                        <div class=""card shadow p-4"" style=""max-width: 400px; width: 100%;"">
                            <h2 class=""text-center mb-4"">Login</h2>
                            <form action=""/Identity/Account/Login"" method=""post"">
                                <input name=""__RequestVerificationToken"" type=""hidden"" value=""{{ANTIFORGERY_TOKEN}}"" />
                                <div class=""mb-3"">
                                    <label for=""email"" class=""form-label"">Email address</label>
                                    <input type=""email"" class=""form-control"" id=""email"" name=""Input.Email"" required />
                                </div>
                                <div class=""mb-3"">
                                    <label for=""password"" class=""form-label"">Password</label>
                                    <input type=""password"" class=""form-control"" id=""password"" name=""Input.Password"" required />
                                </div>
                                <div class=""mb-3 form-check"">
                                    <input type=""checkbox"" class=""form-check-input"" id=""rememberMe"" name=""Input.RememberMe"" />
                                    <label class=""form-check-label"" for=""rememberMe"">Remember Me</label>
                                </div>
                                <button type=""submit"" class=""btn btn-primary w-100"">Login</button>
                            </form>
                            <div class=""mt-3 text-center"">
                                <a href=""/forgot-password"">Forgot Password?</a><br />
                            </div>
                        </div>
                    </div>",
                LastUpdated = DateTime.UtcNow
            },


            // Logout page content
            new PageContent
                {
                    Id = 3,
                    PageKey = "portal-logout",
                    Title = "Logout",
                    HtmlContent = @"
                <div class=""container mt-5 text-center"">
                    <h2>You have been logged out</h2>
                    <p>Thank you for visiting. See you again soon!</p>
                    <a href=""/"" class=""btn btn-primary mt-3"">Return Home</a>
                </div>",
                    LastUpdated = DateTime.UtcNow
                },
                new PageContent
                {
                    Id = 4,
                    PageKey = "register",
                    Title = "Register",
                    HtmlContent = @"
                    <div class=""container mt-5"">
                        <h2 class=""mb-4"">Register</h2>
                        <form action=""/Identity/Account/Register"" method=""post"">
                            <div class=""mb-3"">
                                <label for=""Input_Email"" class=""form-label"">Email address</label>
                                <input type=""email"" class=""form-control"" id=""Input_Email"" name=""Input.Email"" required />
                            </div>
                            <div class=""mb-3"">
                                <label for=""Input_Password"" class=""form-label"">Password</label>
                                <input type=""password"" class=""form-control"" id=""Input_Password"" name=""Input.Password"" required />
                            </div>
                            <div class=""mb-3"">
                                <label for=""Input_ConfirmPassword"" class=""form-label"">Confirm Password</label>
                                <input type=""password"" class=""form-control"" id=""Input_ConfirmPassword"" name=""Input.ConfirmPassword"" required />
                            </div>
                            <button type=""submit"" class=""btn btn-primary"">Register</button>
                        </form>
                        <div class=""mt-3"">
                            <a href=""/Identity/Account/Login"">Already have an account? Login here</a>
                        </div>
                    </div>",
                    LastUpdated = DateTime.UtcNow
                },
                new PageContent
                {
                    Id = 5,
                    PageKey = "forgot-password",
                    Title = "Forgot Password",
                    HtmlContent = @"
                <div class=""container mt-5"">
                    <h2 class=""mb-4"">Forgot your password?</h2>
                    <form action=""/Identity/Account/ForgotPassword"" method=""post"">
                        <div class=""mb-3"">
                            <label for=""Input_Email"" class=""form-label"">Email address</label>
                            <input type=""email"" class=""form-control"" id=""Input_Email"" name=""Input.Email"" required />
                        </div>
                        <button type=""submit"" class=""btn btn-warning"">Send Password Reset Link</button>
                    </form>
                    <div class=""mt-3"">
                        <a href=""/Identity/Account/Login"">Back to Login</a>
                    </div>
                </div>",
                    LastUpdated = DateTime.UtcNow
                },
                new PageContent
                {
                    Id = 6,
                    PageKey = "reset-password",
                    Title = "Reset Password",
                    HtmlContent = @"
                <div class=""container mt-5"">
                    <h2 class=""mb-4"">Reset your password</h2>
                    <form action=""/Identity/Account/ResetPassword"" method=""post"">
                        <div class=""mb-3"">
                            <label for=""Input_Email"" class=""form-label"">Email address</label>
                            <input type=""email"" class=""form-control"" id=""Input_Email"" name=""Input.Email"" required />
                        </div>
                        <div class=""mb-3"">
                            <label for=""Input_Password"" class=""form-label"">New Password</label>
                            <input type=""password"" class=""form-control"" id=""Input_Password"" name=""Input.Password"" required />
                        </div>
                        <div class=""mb-3"">
                            <label for=""Input_ConfirmPassword"" class=""form-label"">Confirm New Password</label>
                            <input type=""password"" class=""form-control"" id=""Input_ConfirmPassword"" name=""Input.ConfirmPassword"" required />
                        </div>
                        <button type=""submit"" class=""btn btn-success"">Reset Password</button>
                    </form>
                </div>",
                    LastUpdated = DateTime.UtcNow
                },
                new PageContent
                {
                    Id = 7,
                    PageKey = "change-password",
                    Title = "Change Password",
                    HtmlContent = @"
                    <div class=""container mt-5"">
                    <h2 class=""mb-4"">Change your password</h2>
                    <form action=""/Identity/Account/Manage/ChangePassword"" method=""post"">
                        <div class=""mb-3"">
                            <label for=""Input_OldPassword"" class=""form-label"">Current Password</label>
                            <input type=""password"" class=""form-control"" id=""Input_OldPassword"" name=""Input.OldPassword"" required />
                        </div>
                        <div class=""mb-3"">
                            <label for=""Input_NewPassword"" class=""form-label"">New Password</label>
                            <input type=""password"" class=""form-control"" id=""Input_NewPassword"" name=""Input.NewPassword"" required />
                        </div>
                        <div class=""mb-3"">
                            <label for=""Input_ConfirmPassword"" class=""form-label"">Confirm New Password</label>
                            <input type=""password"" class=""form-control"" id=""Input_ConfirmPassword"" name=""Input.ConfirmPassword"" required />
                        </div>
                        <button type=""submit"" class=""btn btn-success"">Change Password</button>
                    </form>
                </div>",
                    LastUpdated = DateTime.UtcNow
                },
                new PageContent
                {
                    Id = 8,
                    PageKey = "admin/settings",
                    Title = "Site Settings",
                    HtmlContent = @"
                    <div class=""container mt-5"">
                        <h2 class=""mb-4"">Edit Site Settings</h2>
                        <form action=""/admin/settings/save"" method=""post"">
                            <div class=""mb-3"">
                                <label for=""SiteName"" class=""form-label"">Site Name</label>
                                <input type=""text"" class=""form-control"" id=""SiteName"" name=""SiteSettings.SiteName"" required />
                            </div>
                            <div class=""mb-3"">
                                <label for=""Tagline"" class=""form-label"">Tagline</label>
                                <input type=""text"" class=""form-control"" id=""Tagline"" name=""SiteSettings.Tagline"" />
                            </div>
                            <div class=""mb-3"">
                                <label for=""ThemeId"" class=""form-label"">Theme</label>
                                <select class=""form-select"" id=""ThemeId"" name=""SiteSettings.ThemeId"">
                                    {{#each Themes}}
                                    <option value=""{{Id}}"" {{#if IsSelected}}selected{{/if}}>{{DisplayName}}</option>
                                    {{/each}}
                                </select>
                            </div>
                            <div class=""mb-3"">
                                <label for=""ContactEmail"" class=""form-label"">Contact Email</label>
                                <input type=""email"" class=""form-control"" id=""ContactEmail"" name=""SiteSettings.ContactEmail"" />
                            </div>
                            <div class=""mb-3"">
                                <label for=""ContactPhone"" class=""form-label"">Contact Phone</label>
                                <input type=""text"" class=""form-control"" id=""ContactPhone"" name=""SiteSettings.ContactPhone"" />
                            </div>
                            <div class=""mb-3"">
                                <label for=""ContactAddress"" class=""form-label"">Contact Address</label>
                                <input type=""text"" class=""form-control"" id=""ContactAddress"" name=""SiteSettings.ContactAddress"" />
                            </div>
                            <div class=""mb-3"">
                                <label for=""Copyright"" class=""form-label"">Copyright</label>
                                <input type=""text"" class=""form-control"" id=""Copyright"" name=""SiteSettings.Copyright"" />
                            </div>
                            <div class=""mb-3"">
                                <label for=""SocialLinks"" class=""form-label"">Social Links</label>
                                <input type=""text"" class=""form-control"" id=""SocialLinks"" name=""SiteSettings.SocialLinks"" value=""TODO: Build link editor"" />
                            </div>
                            <button type=""submit"" class=""btn btn-primary"">Save Changes</button>
                        </form>
                    </div>",
                    LastUpdated = DateTime.UtcNow
                }

            );
        }

        private static void SeedDefaultData(ModelBuilder modelBuilder)
        {
            // Theme Settings
            modelBuilder.Entity<ThemeSetting>().HasData(new ThemeSetting
            {
                Id = 1,
                Name = "default",
                DisplayName = "Default Theme",
                Description = "Clean layout with light styling",
                PreviewImage = "/Themes/Default/preview.png",
                SetOn = DateTime.UtcNow
            });

            // Site Settings
            modelBuilder.Entity<SiteSettings>().HasData(new SiteSettings
            {
                Id = 1,
                SiteName = "SCMS Site",
                Tagline = "Powered by SCMS",
                ThemeId = 1
            });

            modelBuilder.Entity<SocialMedia>().HasData(
                new SocialMedia { Id = 1, Name = "Facebook", Url = "#", IconClass = "fab fa-facebook-f", SiteSettingsId = 1 },
                new SocialMedia { Id = 2, Name = "Twitter", Url = "#", IconClass = "fab fa-twitter", SiteSettingsId = 1 },
                new SocialMedia { Id = 3, Name = "Instagram", Url = "#", IconClass = "fab fa-instagram", SiteSettingsId = 1 },
                new SocialMedia { Id = 4, Name = "YouTube", Url = "#", IconClass = "fab fa-youtube", SiteSettingsId = 1 },
                new SocialMedia { Id = 5, Name = "LinkedIn", Url = "#", IconClass = "fab fa-linkedin-in", SiteSettingsId = 1 },
                new SocialMedia { Id = 6, Name = "TikTok", Url = "#", IconClass = "fab fa-tiktok", SiteSettingsId = 1 },
                new SocialMedia { Id = 7, Name = "Pinterest", Url = "#", IconClass = "fab fa-pinterest-p", SiteSettingsId = 1 },
                new SocialMedia { Id = 8, Name = "Bluesky", Url = "#", IconClass = "fas fa-globe", SiteSettingsId = 1 }
            );


            // Default Home Page Content
            modelBuilder.Entity<PageContent>().HasData(new PageContent
            {
                Id = 1,
                PageKey = "home",
                Title = "Welcome",
                HtmlContent = @"
                    <h1 class=""display-4 mb-4"">Welcome</h1>
                    <p class=""lead mb-4"">This is your first SCMS page. Edit it in the admin panel.</p>",                    
                LastUpdated = DateTime.UtcNow
            });

            // Main Menu Items
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 100,
                    ParentId = null,
                    Title = "Home",
                    Url = null,
                    PageContentId = 1,
                    MenuGroup = "Main",
                    Order = 0,
                    IsVisible = true,
                    SecurityLevelId = 3
                },
                new MenuItem
                {
                    Id = 1,
                    ParentId = null,
                    Title = "Admin",
                    Url = "#",
                    PageContentId = null,
                    MenuGroup = "Main",
                    Order = 99,
                    IsVisible = true,
                    SecurityLevelId = 1
                },
                new MenuItem
                {
                    Id = 2,
                    ParentId = 1,
                    Title = "Site Settings",
                    Url = "/admin/settings",
                    PageContentId = 8,
                    MenuGroup = "Main",
                    Order = 0,
                    IsVisible = true,
                    SecurityLevelId = 1
                }
            );


            modelBuilder.Entity<SecurityLevel>().HasData(
                new SecurityLevel { Id = 1, Name = "Administrator", Description = "Can modify site settings.", IsSystem = true },
                new SecurityLevel { Id = 2, Name = "User", Description = "Registered user for protected pages.", IsSystem = true },
                new SecurityLevel { Id = 3, Name = "Anonymous", Description = "Public access level.", IsSystem = true }
            );
            modelBuilder.Entity<SecurityLevelRole>().HasData(
                new SecurityLevelRole { Id = 1, SecurityLevelId = 1, RoleName = "Administrator" },
                new SecurityLevelRole { Id = 2, SecurityLevelId = 2, RoleName = "User" }
                // Anonymous is handled as fallback when no auth roles are matched
            );
            

        }
    }
}
