using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Presentation.Migrations.TtcDb
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Week = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    Genre = table.Column<string>(nullable: true),
                    HeaderVideoUrl = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Event = table.Column<string>(nullable: true),
                    EventDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageLink = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TtcUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TtcUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    VideoUri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Artiste = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false),
                    LastPosition = table.Column<int>(nullable: false),
                    ImageUri = table.Column<string>(nullable: true),
                    HighestPosition = table.Column<int>(nullable: false),
                    ChartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartItems_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    TtcUserId = table.Column<int>(nullable: false),
                    HeaderImageUri = table.Column<string>(nullable: true),
                    NewsContent = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsItems_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    TtcUserId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    HeaderImage = table.Column<string>(nullable: true),
                    IsToDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoItems_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartItems_ChartId",
                table: "ChartItems",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsItems_TtcUserId",
                table: "NewsItems",
                column: "TtcUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoItems_TtcUserId",
                table: "PhotoItems",
                column: "TtcUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartItems");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MediaItems");

            migrationBuilder.DropTable(
                name: "NewsItems");

            migrationBuilder.DropTable(
                name: "PhotoItems");

            migrationBuilder.DropTable(
                name: "VideoItems");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "TtcUsers");
        }
    }
}
