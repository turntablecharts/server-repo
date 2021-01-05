using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class addingcontentposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticlePosition",
                table: "MagazineDatas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticlePosition",
                table: "MagazineDatas");
        }
    }
}
