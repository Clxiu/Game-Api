using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameApi.Migrations
{
    public partial class createTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leaderboard",
                columns: table => new
                {
                    GameLeaderboardEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameInstanceId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboard", x => x.GameLeaderboardEntryId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GameInstance",
                columns: table => new
                {
                    GameInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameActive = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    RowSize = table.Column<int>(type: "int", nullable: false),
                    ColumnSize = table.Column<int>(type: "int", nullable: false),
                    LatestGrid = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInstance", x => x.GameInstanceId);
                    table.ForeignKey(
                        name: "FK_GameInstance_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameStep",
                columns: table => new
                {
                    GameStepId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameInstanceId = table.Column<int>(type: "int", nullable: false),
                    NewTileRow = table.Column<int>(type: "int", nullable: false),
                    NewTileColumn = table.Column<int>(type: "int", nullable: false),
                    NewTileExponent = table.Column<int>(type: "int", nullable: false),
                    LastMove = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStep", x => x.GameStepId);
                    table.ForeignKey(
                        name: "FK_GameStep_GameInstance_GameInstanceId",
                        column: x => x.GameInstanceId,
                        principalTable: "GameInstance",
                        principalColumn: "GameInstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInstance_UserId",
                table: "GameInstance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStep_GameInstanceId",
                table: "GameStep",
                column: "GameInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStep");

            migrationBuilder.DropTable(
                name: "Leaderboard");

            migrationBuilder.DropTable(
                name: "GameInstance");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
