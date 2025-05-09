using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcLab9.Migrations
{
    /// <inheritdoc />
    public partial class MakeLoginUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_login",
                table: "Users",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_login",
                table: "Users");
        }
    }
}
