using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordIterations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordIterations",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordIterations",
                table: "Users");
        }
    }
}
