using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class newsCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas");

            migrationBuilder.AlterColumn<int>(
                name: "MagazineEditionDataId",
                table: "MagazineDatas",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NewsCategoryDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategoryDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    TtcUserId = table.Column<int>(nullable: false),
                    HeaderImageUri = table.Column<string>(nullable: true),
                    NewsContent = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    NewsCategoryDataId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsDatas_NewsCategoryDatas_NewsCategoryDataId",
                        column: x => x.NewsCategoryDataId,
                        principalTable: "NewsCategoryDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsDatas_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsDatas_NewsCategoryDataId",
                table: "NewsDatas",
                column: "NewsCategoryDataId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsDatas_TtcUserId",
                table: "NewsDatas",
                column: "TtcUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas",
                column: "MagazineEditionDataId",
                principalTable: "MagazineEditionDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas");

            migrationBuilder.DropTable(
                name: "NewsDatas");

            migrationBuilder.DropTable(
                name: "NewsCategoryDatas");

            migrationBuilder.AlterColumn<int>(
                name: "MagazineEditionDataId",
                table: "MagazineDatas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas",
                column: "MagazineEditionDataId",
                principalTable: "MagazineEditionDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
