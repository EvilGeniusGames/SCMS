using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c98496d2-b649-44d2-badd-d667690a33dc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dcec7256-e937-448c-ac49-06cd97906a43");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e01a9087-f6e9-41d1-9c7d-54f4a40b7f8a");

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
                    SetOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThemeSettings", x => x.Id);
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
                name: "SocialMedia",
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
                    table.PrimaryKey("PK_SocialMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMedia_SiteSettings_SiteSettingsId",
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
                    { "21f07b18-2fa9-4dd4-af05-ffc09f664e2f", null, "Admin", "ADMIN" },
                    { "c16b2241-1c29-4c4c-8cef-acc01af12376", null, "Editor", "EDITOR" },
                    { "c368209b-1e55-4a08-92cf-20b2ddc2845e", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "PageContents",
                columns: new[] { "Id", "HtmlContent", "LastUpdated", "MetaDescription", "MetaKeywords", "PageKey", "TemplateKey", "Title", "UpdatedBy", "Visibility" },
                values: new object[] { 1, "<p>This is your first SCMS page. Edit it in the admin panel.</p>", new DateTime(2025, 4, 24, 18, 33, 21, 757, DateTimeKind.Utc).AddTicks(7880), null, null, "home", "Display", "Welcome", null, "Public" });

            migrationBuilder.InsertData(
                table: "ThemeSettings",
                columns: new[] { "Id", "Description", "DisplayName", "Name", "PreviewImage", "SetOn" },
                values: new object[] { 1, "Clean layout with light styling", "Default Theme", "default", "/Themes/Default/preview.png", new DateTime(2025, 4, 24, 18, 33, 21, 757, DateTimeKind.Utc).AddTicks(7835) });

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Id", "ContactAddress", "ContactEmail", "ContactPhone", "Logo", "SiteName", "Tagline", "ThemeId" },
                values: new object[] { 1, null, null, null, null, "SCMS Site", "Powered by SCMS", 1 });

            migrationBuilder.InsertData(
                table: "SocialMedia",
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
                name: "IX_SiteSettings_ThemeId",
                table: "SiteSettings",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMedia_SiteSettingsId",
                table: "SocialMedia",
                column: "SiteSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "SocialMedia");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "ThemeSettings");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21f07b18-2fa9-4dd4-af05-ffc09f664e2f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c16b2241-1c29-4c4c-8cef-acc01af12376");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c368209b-1e55-4a08-92cf-20b2ddc2845e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c98496d2-b649-44d2-badd-d667690a33dc", null, "Editor", "EDITOR" },
                    { "dcec7256-e937-448c-ac49-06cd97906a43", null, "User", "USER" },
                    { "e01a9087-f6e9-41d1-9c7d-54f4a40b7f8a", null, "Admin", "ADMIN" }
                });
        }
    }
}
