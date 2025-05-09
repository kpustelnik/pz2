using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcLab9.Migrations
{
    /// <inheritdoc />
    public partial class MakeIndexInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "login", "password" },
                values: new object[] { 1, "admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "login", "password" },
                values: new object[] { 1L, "admin", "admin" });
        }
    }
}
