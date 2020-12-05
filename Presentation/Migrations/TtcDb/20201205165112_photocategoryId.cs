using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class photocategoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoDatas_PhotoCategoryDatas_PhotoCategoryDataId",
                table: "PhotoDatas");

            migrationBuilder.AlterColumn<int>(
                name: "PhotoCategoryDataId",
                table: "PhotoDatas",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoDatas_PhotoCategoryDatas_PhotoCategoryDataId",
                table: "PhotoDatas",
                column: "PhotoCategoryDataId",
                principalTable: "PhotoCategoryDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoDatas_PhotoCategoryDatas_PhotoCategoryDataId",
                table: "PhotoDatas");

            migrationBuilder.AlterColumn<int>(
                name: "PhotoCategoryDataId",
                table: "PhotoDatas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoDatas_PhotoCategoryDatas_PhotoCategoryDataId",
                table: "PhotoDatas",
                column: "PhotoCategoryDataId",
                principalTable: "PhotoCategoryDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
