using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class photoMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoCategoryDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoCategoryDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    TtcUserId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    HeaderImage = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    PhotoCategoryDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoDatas_PhotoCategoryDatas_PhotoCategoryDataId",
                        column: x => x.PhotoCategoryDataId,
                        principalTable: "PhotoCategoryDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoDatas_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDatas_PhotoCategoryDataId",
                table: "PhotoDatas",
                column: "PhotoCategoryDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDatas_TtcUserId",
                table: "PhotoDatas",
                column: "TtcUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoDatas");

            migrationBuilder.DropTable(
                name: "PhotoCategoryDatas");
        }
    }
}
