using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class adding_chartHIghlight_databmodelingl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartHighlights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Artiste = table.Column<string>(nullable: true),
                    ImageUri = table.Column<string>(nullable: true),
                    LastPosition = table.Column<int>(nullable: false),
                    HighestPosition = table.Column<int>(nullable: false),
                    MusicLink = table.Column<string>(nullable: true),
                    ChartHighlightType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartHighlights", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartHighlights");
        }
    }
}
