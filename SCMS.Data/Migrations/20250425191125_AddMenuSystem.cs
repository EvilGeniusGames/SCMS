using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "47cf3423-6cda-4c0c-a55b-201481281e36");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f5f01f6-2649-4b7c-ba32-c78a638be20c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93766676-605f-4c15-b7e8-df3f76c891ed");

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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13547119-eab2-4a1b-ac55-3034c77677c2", null, "Editor", "EDITOR" },
                    { "8fb905cb-5e9a-46d5-ba4f-3a62a645cf1d", null, "Admin", "ADMIN" },
                    { "ef0e0fad-f86d-4238-864e-9f31c70e8d00", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "IsVisible", "MenuGroup", "Order", "PageContentId", "ParentId", "Title", "Url" },
                values: new object[] { 1, true, "Main", 0, 1, null, "Home", null });

            migrationBuilder.UpdateData(
                table: "PageContents",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 25, 19, 11, 25, 384, DateTimeKind.Utc).AddTicks(4535));

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SetOn",
                value: new DateTime(2025, 4, 25, 19, 11, 25, 384, DateTimeKind.Utc).AddTicks(4487));

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PageContentId",
                table: "MenuItems",
                column: "PageContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13547119-eab2-4a1b-ac55-3034c77677c2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fb905cb-5e9a-46d5-ba4f-3a62a645cf1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef0e0fad-f86d-4238-864e-9f31c70e8d00");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "47cf3423-6cda-4c0c-a55b-201481281e36", null, "Editor", "EDITOR" },
                    { "5f5f01f6-2649-4b7c-ba32-c78a638be20c", null, "User", "USER" },
                    { "93766676-605f-4c15-b7e8-df3f76c891ed", null, "Admin", "ADMIN" }
                });

            migrationBuilder.UpdateData(
                table: "PageContents",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 25, 15, 4, 23, 626, DateTimeKind.Utc).AddTicks(6435));

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SetOn",
                value: new DateTime(2025, 4, 25, 15, 4, 23, 626, DateTimeKind.Utc).AddTicks(6378));
        }
    }
}
