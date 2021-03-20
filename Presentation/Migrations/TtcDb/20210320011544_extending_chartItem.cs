using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class extending_chartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProducedBy",
                table: "ChartItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeeksOnChart",
                table: "ChartItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProducedBy",
                table: "ChartItems");

            migrationBuilder.DropColumn(
                name: "WeeksOnChart",
                table: "ChartItems");
        }
    }
}
