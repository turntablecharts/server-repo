using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class magazineeditiondata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagazineDatas_MagazineEditions_MagazineEditionId",
                table: "MagazineDatas");

            migrationBuilder.DropTable(
                name: "MagazineEditions");

            migrationBuilder.DropIndex(
                name: "IX_MagazineDatas_MagazineEditionId",
                table: "MagazineDatas");

            migrationBuilder.AddColumn<int>(
                name: "MagazineEditionDataId",
                table: "MagazineDatas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MagazineEditionDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineEditionDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MagazineDatas_MagazineEditionDataId",
                table: "MagazineDatas",
                column: "MagazineEditionDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas",
                column: "MagazineEditionDataId",
                principalTable: "MagazineEditionDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                table: "MagazineDatas");

            migrationBuilder.DropTable(
                name: "MagazineEditionDatas");

            migrationBuilder.DropIndex(
                name: "IX_MagazineDatas_MagazineEditionDataId",
                table: "MagazineDatas");

            migrationBuilder.DropColumn(
                name: "MagazineEditionDataId",
                table: "MagazineDatas");

            migrationBuilder.CreateTable(
                name: "MagazineEditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineEditions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MagazineDatas_MagazineEditionId",
                table: "MagazineDatas",
                column: "MagazineEditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MagazineDatas_MagazineEditions_MagazineEditionId",
                table: "MagazineDatas",
                column: "MagazineEditionId",
                principalTable: "MagazineEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
