using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presentation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePowerlistRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertifiedSongs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Milestone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Artiste = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsClaimed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertifiedSongs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartHighlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Artiste = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPosition = table.Column<int>(type: "int", nullable: false),
                    HighestPosition = table.Column<int>(type: "int", nullable: false),
                    MusicLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartHighlightType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartHighlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Week = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderVideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ChartCategoryId = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GalleryType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MagazineEditionDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineEditionDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerlistCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerlistCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerlistEditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerlistEditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerListNominations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Achievement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerListNominations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscribersEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignUpDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribersEmails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TtcUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TtcUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Artiste = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPosition = table.Column<int>(type: "int", nullable: false),
                    HighestPosition = table.Column<int>(type: "int", nullable: false),
                    MusicLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeeksOnChart = table.Column<int>(type: "int", nullable: false),
                    ProducedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartItems_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PowerlistRecognitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PowerlistEditionId = table.Column<int>(type: "int", nullable: false),
                    PowerlistCategoryId = table.Column<int>(type: "int", nullable: false),
                    Office = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerlistRecognitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerlistRecognitions_PowerlistCategories_PowerlistCategoryId",
                        column: x => x.PowerlistCategoryId,
                        principalTable: "PowerlistCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PowerlistRecognitions_PowerlistEditions_PowerlistEditionId",
                        column: x => x.PowerlistEditionId,
                        principalTable: "PowerlistEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MagazineDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TtcUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MagazineEditionId = table.Column<int>(type: "int", nullable: false),
                    MagazineEditionDataId = table.Column<int>(type: "int", nullable: false),
                    ArticlePosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagazineDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagazineDatas_MagazineEditionDatas_MagazineEditionDataId",
                        column: x => x.MagazineEditionDataId,
                        principalTable: "MagazineEditionDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MagazineDatas_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TtcUserId = table.Column<int>(type: "int", nullable: false),
                    HeaderImageUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewsContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewsCategoryId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_NewsCategories_NewsCategoryId",
                        column: x => x.NewsCategoryId,
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_News_TtcUsers_TtcUserId",
                        column: x => x.TtcUserId,
                        principalTable: "TtcUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TtcUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_PhotoCategories_PhotoCategoryId",
                        column: x => x.PhotoCategoryId,
                        principalTable: "PhotoCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photos_TtcUsers_TtcUserId",
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
                name: "IX_MagazineDatas_MagazineEditionDataId",
                table: "MagazineDatas",
                column: "MagazineEditionDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MagazineDatas_TtcUserId",
                table: "MagazineDatas",
                column: "TtcUserId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_News_TtcUserId",
                table: "News",
                column: "TtcUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoCategoryId",
                table: "Photos",
                column: "PhotoCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_TtcUserId",
                table: "Photos",
                column: "TtcUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerlistRecognitions_PowerlistCategoryId",
                table: "PowerlistRecognitions",
                column: "PowerlistCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerlistRecognitions_PowerlistEditionId",
                table: "PowerlistRecognitions",
                column: "PowerlistEditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertifiedSongs");

            migrationBuilder.DropTable(
                name: "ChartCategories");

            migrationBuilder.DropTable(
                name: "ChartHighlights");

            migrationBuilder.DropTable(
                name: "ChartItems");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MagazineDatas");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "PowerListNominations");

            migrationBuilder.DropTable(
                name: "PowerlistRecognitions");

            migrationBuilder.DropTable(
                name: "SubscribersEmails");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "MagazineEditionDatas");

            migrationBuilder.DropTable(
                name: "NewsCategories");

            migrationBuilder.DropTable(
                name: "PhotoCategories");

            migrationBuilder.DropTable(
                name: "TtcUsers");

            migrationBuilder.DropTable(
                name: "PowerlistCategories");

            migrationBuilder.DropTable(
                name: "PowerlistEditions");
        }
    }
}
