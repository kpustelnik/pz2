using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcLab9.Migrations
{
    /// <inheritdoc />
    public partial class InitialUsersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "login", "password" },
                values: new object[] { 1L, "admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
