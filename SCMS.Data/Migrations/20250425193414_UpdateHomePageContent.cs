using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomePageContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    { "27870e25-7a82-400c-b54d-a91324950c32", null, "Editor", "EDITOR" },
                    { "34812801-dcf5-4157-8f25-aeebe9948aed", null, "Admin", "ADMIN" },
                    { "a1df3403-455a-415e-a8bb-f89981e6ff45", null, "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "PageContents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HtmlContent", "LastUpdated" },
                values: new object[] { "\r\n                    <p>This is your first SCMS page. Edit it in the admin panel.</p>\r\n                    <form action=\"/seed-sample-content\" method=\"post\">\r\n                        <button type=\"submit\" class=\"btn btn-warning\">Seed Sample Content</button>\r\n                    </form>", new DateTime(2025, 4, 25, 19, 34, 14, 163, DateTimeKind.Utc).AddTicks(6634) });

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SetOn",
                value: new DateTime(2025, 4, 25, 19, 34, 14, 163, DateTimeKind.Utc).AddTicks(6584));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "27870e25-7a82-400c-b54d-a91324950c32");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34812801-dcf5-4157-8f25-aeebe9948aed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1df3403-455a-415e-a8bb-f89981e6ff45");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13547119-eab2-4a1b-ac55-3034c77677c2", null, "Editor", "EDITOR" },
                    { "8fb905cb-5e9a-46d5-ba4f-3a62a645cf1d", null, "Admin", "ADMIN" },
                    { "ef0e0fad-f86d-4238-864e-9f31c70e8d00", null, "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "PageContents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HtmlContent", "LastUpdated" },
                values: new object[] { "<p>This is your first SCMS page. Edit it in the admin panel.</p>", new DateTime(2025, 4, 25, 19, 11, 25, 384, DateTimeKind.Utc).AddTicks(4535) });

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SetOn",
                value: new DateTime(2025, 4, 25, 19, 11, 25, 384, DateTimeKind.Utc).AddTicks(4487));
        }
    }
}
