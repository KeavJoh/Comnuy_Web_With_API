using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComnuyWebWithAPI.Data.Migrations
{
    public partial class addUserAndToolColumnInFavoritesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToolId",
                table: "ToolFavoritesFromUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ToolFavoritesFromUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToolId",
                table: "ToolFavoritesFromUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ToolFavoritesFromUsers");
        }
    }
}
