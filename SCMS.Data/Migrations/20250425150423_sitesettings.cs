using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SCMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class sitesettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Favicon",
                table: "ThemeSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);

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
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Copyright",
                value: null);

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Favicon", "SetOn" },
                values: new object[] { "favicon.ico", new DateTime(2025, 4, 25, 15, 4, 23, 626, DateTimeKind.Utc).AddTicks(6378) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Favicon",
                table: "ThemeSettings");

            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "SiteSettings");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21f07b18-2fa9-4dd4-af05-ffc09f664e2f", null, "Admin", "ADMIN" },
                    { "c16b2241-1c29-4c4c-8cef-acc01af12376", null, "Editor", "EDITOR" },
                    { "c368209b-1e55-4a08-92cf-20b2ddc2845e", null, "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "PageContents",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 24, 18, 33, 21, 757, DateTimeKind.Utc).AddTicks(7880));

            migrationBuilder.UpdateData(
                table: "ThemeSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "SetOn",
                value: new DateTime(2025, 4, 24, 18, 33, 21, 757, DateTimeKind.Utc).AddTicks(7835));
        }
    }
}
