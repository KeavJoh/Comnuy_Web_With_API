using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComnuyWebWithAPI.Data.Migrations
{
    public partial class changeUserIdToUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ToolFavoritesFromUsers",
                newName: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ToolFavoritesFromUsers",
                newName: "UserId");
        }
    }
}
