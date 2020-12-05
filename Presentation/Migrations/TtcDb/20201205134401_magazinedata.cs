using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class magazinedata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MagazineEditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineEditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagazineDatas",
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
                    MagazineEditionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagazineDatas_MagazineEditions_MagazineEditionId",
                        column: x => x.MagazineEditionId,
                        principalTable: "MagazineEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagazineDatas_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MagazineDatas_MagazineEditionId",
                table: "MagazineDatas",
                column: "MagazineEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_MagazineDatas_TtcUserId",
                table: "MagazineDatas",
                column: "TtcUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MagazineDatas");

            migrationBuilder.DropTable(
                name: "MagazineEditions");
        }
    }
}
