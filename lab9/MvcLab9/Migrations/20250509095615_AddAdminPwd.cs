using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcLab9.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminPwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userId = table.Column<int>(type: "INTEGER", nullable: false),
                    data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Data_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "password",
                value: "21232f297a57a5a743894a0e4a801fc3");

            migrationBuilder.CreateIndex(
                name: "IX_Data_userId",
                table: "Data",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "password",
                value: "admin");
        }
    }
}
