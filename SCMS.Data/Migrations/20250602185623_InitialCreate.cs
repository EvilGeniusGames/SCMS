using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageKey = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    HtmlContent = table.Column<string>(type: "TEXT", nullable: false),
                    MetaDescription = table.Column<string>(type: "TEXT", nullable: true),
                    MetaKeywords = table.Column<string>(type: "TEXT", nullable: true),
                    Visibility = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateKey = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThemeSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PreviewImage = table.Column<string>(type: "TEXT", nullable: true),
                    Favicon = table.Column<string>(type: "TEXT", nullable: true),
                    SetOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThemeSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    PageContentId = table.Column<int>(type: "INTEGER", nullable: true),
                    MenuGroup = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_PageContents_PageContentId",
                        column: x => x.PageContentId,
                        principalTable: "PageContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteName = table.Column<string>(type: "TEXT", nullable: false),
                    Tagline = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: true),
                    ContactAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Copyright = table.Column<string>(type: "TEXT", nullable: true),
                    ThemeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteSettings_ThemeSettings_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "ThemeSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialMedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    IconClass = table.Column<string>(type: "TEXT", nullable: true),
                    SiteSettingsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMedias_SiteSettings_SiteSettingsId",
                        column: x => x.SiteSettingsId,
                        principalTable: "SiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1973a127-9431-4bba-87a7-a0abe88cf796", null, "User", "USER" },
                    { "3c02aaab-c3b0-4e75-8dcc-ab6836237572", null, "Admin", "ADMIN" },
                    { "3c57bed7-e68a-45ae-9b9e-b3a0e4eaa2b6", null, "Editor", "EDITOR" }
                });

            migrationBuilder.InsertData(
                table: "PageContents",
                columns: new[] { "Id", "HtmlContent", "LastUpdated", "MetaDescription", "MetaKeywords", "PageKey", "TemplateKey", "Title", "UpdatedBy", "Visibility" },
                values: new object[,]
                {
                    { 1, "\r\n                    <h1 class=\"display-4 mb-4\">Welcome</h1>\r\n                    <p class=\"lead mb-4\">This is your first SCMS page. Edit it in the admin panel.</p>\r\n                    <form action=\"/seed-sample-content\" method=\"post\">\r\n                        <button type=\"submit\" class=\"btn btn-warning\">Seed Sample Content</button>\r\n                    </form>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6015), null, null, "home", "Display", "Welcome", null, "Public" },
                    { 2, "\r\n                    <div class=\"d-flex align-items-center justify-content-center\">\r\n                        <div class=\"card shadow p-4\" style=\"max-width: 400px; width: 100%;\">\r\n                            <h2 class=\"text-center mb-4\">Login</h2>\r\n                            <form action=\"/Identity/Account/Login\" method=\"post\">\r\n                                <input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"{{ANTIFORGERY_TOKEN}}\" />\r\n                                <div class=\"mb-3\">\r\n                                    <label for=\"email\" class=\"form-label\">Email address</label>\r\n                                    <input type=\"email\" class=\"form-control\" id=\"email\" name=\"Input.Email\" required />\r\n                                </div>\r\n                                <div class=\"mb-3\">\r\n                                    <label for=\"password\" class=\"form-label\">Password</label>\r\n                                    <input type=\"password\" class=\"form-control\" id=\"password\" name=\"Input.Password\" required />\r\n                                </div>\r\n                                <div class=\"mb-3 form-check\">\r\n                                    <input type=\"checkbox\" class=\"form-check-input\" id=\"rememberMe\" name=\"Input.RememberMe\" />\r\n                                    <label class=\"form-check-label\" for=\"rememberMe\">Remember Me</label>\r\n                                </div>\r\n                                <button type=\"submit\" class=\"btn btn-primary w-100\">Login</button>\r\n                            </form>\r\n                            <div class=\"mt-3 text-center\">\r\n                                <a href=\"/forgot-password\">Forgot Password?</a><br />\r\n                                <a href=\"/register\">Don't have an account? Register</a>\r\n                            </div>\r\n                        </div>\r\n                    </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6039), null, null, "portal-access", "Display", "Login", null, "Public" },
                    { 3, "\r\n                <div class=\"container mt-5 text-center\">\r\n                    <h2>You have been logged out</h2>\r\n                    <p>Thank you for visiting. See you again soon!</p>\r\n                    <a href=\"/\" class=\"btn btn-primary mt-3\">Return Home</a>\r\n                </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6040), null, null, "portal-logout", "Display", "Logout", null, "Public" },
                    { 4, "\r\n                    <div class=\"container mt-5\">\r\n                        <h2 class=\"mb-4\">Register</h2>\r\n                        <form action=\"/Identity/Account/Register\" method=\"post\">\r\n                            <div class=\"mb-3\">\r\n                                <label for=\"Input_Email\" class=\"form-label\">Email address</label>\r\n                                <input type=\"email\" class=\"form-control\" id=\"Input_Email\" name=\"Input.Email\" required />\r\n                            </div>\r\n                            <div class=\"mb-3\">\r\n                                <label for=\"Input_Password\" class=\"form-label\">Password</label>\r\n                                <input type=\"password\" class=\"form-control\" id=\"Input_Password\" name=\"Input.Password\" required />\r\n                            </div>\r\n                            <div class=\"mb-3\">\r\n                                <label for=\"Input_ConfirmPassword\" class=\"form-label\">Confirm Password</label>\r\n                                <input type=\"password\" class=\"form-control\" id=\"Input_ConfirmPassword\" name=\"Input.ConfirmPassword\" required />\r\n                            </div>\r\n                            <button type=\"submit\" class=\"btn btn-primary\">Register</button>\r\n                        </form>\r\n                        <div class=\"mt-3\">\r\n                            <a href=\"/Identity/Account/Login\">Already have an account? Login here</a>\r\n                        </div>\r\n                    </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6041), null, null, "register", "Display", "Register", null, "Public" },
                    { 5, "\r\n                <div class=\"container mt-5\">\r\n                    <h2 class=\"mb-4\">Forgot your password?</h2>\r\n                    <form action=\"/Identity/Account/ForgotPassword\" method=\"post\">\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_Email\" class=\"form-label\">Email address</label>\r\n                            <input type=\"email\" class=\"form-control\" id=\"Input_Email\" name=\"Input.Email\" required />\r\n                        </div>\r\n                        <button type=\"submit\" class=\"btn btn-warning\">Send Password Reset Link</button>\r\n                    </form>\r\n                    <div class=\"mt-3\">\r\n                        <a href=\"/Identity/Account/Login\">Back to Login</a>\r\n                    </div>\r\n                </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6042), null, null, "forgot-password", "Display", "Forgot Password", null, "Public" },
                    { 6, "\r\n                <div class=\"container mt-5\">\r\n                    <h2 class=\"mb-4\">Reset your password</h2>\r\n                    <form action=\"/Identity/Account/ResetPassword\" method=\"post\">\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_Email\" class=\"form-label\">Email address</label>\r\n                            <input type=\"email\" class=\"form-control\" id=\"Input_Email\" name=\"Input.Email\" required />\r\n                        </div>\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_Password\" class=\"form-label\">New Password</label>\r\n                            <input type=\"password\" class=\"form-control\" id=\"Input_Password\" name=\"Input.Password\" required />\r\n                        </div>\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_ConfirmPassword\" class=\"form-label\">Confirm New Password</label>\r\n                            <input type=\"password\" class=\"form-control\" id=\"Input_ConfirmPassword\" name=\"Input.ConfirmPassword\" required />\r\n                        </div>\r\n                        <button type=\"submit\" class=\"btn btn-success\">Reset Password</button>\r\n                    </form>\r\n                </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6043), null, null, "reset-password", "Display", "Reset Password", null, "Public" },
                    { 7, "\r\n                    <div class=\"container mt-5\">\r\n                    <h2 class=\"mb-4\">Change your password</h2>\r\n                    <form action=\"/Identity/Account/Manage/ChangePassword\" method=\"post\">\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_OldPassword\" class=\"form-label\">Current Password</label>\r\n                            <input type=\"password\" class=\"form-control\" id=\"Input_OldPassword\" name=\"Input.OldPassword\" required />\r\n                        </div>\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_NewPassword\" class=\"form-label\">New Password</label>\r\n                            <input type=\"password\" class=\"form-control\" id=\"Input_NewPassword\" name=\"Input.NewPassword\" required />\r\n                        </div>\r\n                        <div class=\"mb-3\">\r\n                            <label for=\"Input_ConfirmPassword\" class=\"form-label\">Confirm New Password</label>\r\n                            <input type=\"password\" class=\"form-control\" id=\"Input_ConfirmPassword\" name=\"Input.ConfirmPassword\" required />\r\n                        </div>\r\n                        <button type=\"submit\" class=\"btn btn-success\">Change Password</button>\r\n                    </form>\r\n                </div>", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(6044), null, null, "change-password", "Display", "Change Password", null, "Public" }
                });

            migrationBuilder.InsertData(
                table: "ThemeSettings",
                columns: new[] { "Id", "Description", "DisplayName", "Favicon", "Name", "PreviewImage", "SetOn" },
                values: new object[] { 1, "Clean layout with light styling", "Default Theme", "favicon.ico", "default", "/Themes/Default/preview.png", new DateTime(2025, 6, 2, 18, 56, 23, 525, DateTimeKind.Utc).AddTicks(5964) });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "IsVisible", "MenuGroup", "Order", "PageContentId", "ParentId", "Title", "Url" },
                values: new object[] { 1, true, "Main", 0, 1, null, "Home", null });

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Id", "ContactAddress", "ContactEmail", "ContactPhone", "Copyright", "Logo", "SiteName", "Tagline", "ThemeId" },
                values: new object[] { 1, null, null, null, null, null, "SCMS Site", "Powered by SCMS", 1 });

            migrationBuilder.InsertData(
                table: "SocialMedias",
                columns: new[] { "Id", "IconClass", "Name", "SiteSettingsId", "Url" },
                values: new object[,]
                {
                    { 1, "fab fa-facebook-f", "Facebook", 1, "#" },
                    { 2, "fab fa-twitter", "Twitter", 1, "#" },
                    { 3, "fab fa-instagram", "Instagram", 1, "#" },
                    { 4, "fab fa-youtube", "YouTube", 1, "#" },
                    { 5, "fab fa-linkedin-in", "LinkedIn", 1, "#" },
                    { 6, "fab fa-tiktok", "TikTok", 1, "#" },
                    { 7, "fab fa-pinterest-p", "Pinterest", 1, "#" },
                    { 8, "fas fa-globe", "Bluesky", 1, "#" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PageContentId",
                table: "MenuItems",
                column: "PageContentId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_ThemeId",
                table: "SiteSettings",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedias_SiteSettingsId",
                table: "SocialMedias",
                column: "SiteSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "SocialMedias");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "ThemeSettings");
        }
    }
}
