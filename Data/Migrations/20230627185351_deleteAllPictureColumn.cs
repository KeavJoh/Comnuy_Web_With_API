using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComnuyWebWithAPI.Data.Migrations
{
    public partial class deleteAllPictureColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture_1",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "Picture_2",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "Picture_3",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "Picture_4",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "Picture_5",
                table: "Tools");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture_1",
                table: "Tools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture_2",
                table: "Tools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture_3",
                table: "Tools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture_4",
                table: "Tools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Picture_5",
                table: "Tools",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
