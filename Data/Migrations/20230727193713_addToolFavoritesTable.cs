using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComnuyWebWithAPI.Data.Migrations
{
    public partial class addToolFavoritesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToolFavoritesFromUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolFavoritesFromUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToolFavoritesFromUsers");
        }
    }
}
