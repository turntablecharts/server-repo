using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class addingcategoy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "VideoItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PhotoItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "NewsItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "VideoItems");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PhotoItems");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "NewsItems");
        }
    }
}
