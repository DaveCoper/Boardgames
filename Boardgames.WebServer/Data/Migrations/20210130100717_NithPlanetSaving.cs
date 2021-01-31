using Microsoft.EntityFrameworkCore.Migrations;

namespace Boardgames.WebServer.Data.Migrations
{
    public partial class NithPlanetSaving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaptainPlayerId",
                table: "NinthPlanetGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColorOfCurrentTrick",
                table: "NinthPlanetGames",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPlayerId",
                table: "NinthPlanetGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RoundIsInProgress",
                table: "NinthPlanetGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NinthPlanetPlayerStates",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    SerializedCardsInHand = table.Column<string>(type: "TEXT", nullable: true),
                    SerializedUnfinishedTasks = table.Column<string>(type: "TEXT", nullable: true),
                    SerializedFinishedTasks = table.Column<string>(type: "TEXT", nullable: true),
                    ComunicatedCardColor = table.Column<int>(type: "INTEGER", nullable: true),
                    ComunicatedCardValue = table.Column<int>(type: "INTEGER", nullable: true),
                    CommunicationTokenPosition = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NinthPlanetPlayerStates", x => new { x.PlayerId, x.GameId });
                    table.ForeignKey(
                        name: "FK_NinthPlanetPlayerStates_AspNetUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NinthPlanetPlayerStates_NinthPlanetGames_GameId",
                        column: x => x.GameId,
                        principalTable: "NinthPlanetGames",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NinthPlanetPlayerStates_GameId",
                table: "NinthPlanetPlayerStates",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NinthPlanetPlayerStates");

            migrationBuilder.DropColumn(
                name: "CaptainPlayerId",
                table: "NinthPlanetGames");

            migrationBuilder.DropColumn(
                name: "ColorOfCurrentTrick",
                table: "NinthPlanetGames");

            migrationBuilder.DropColumn(
                name: "CurrentPlayerId",
                table: "NinthPlanetGames");

            migrationBuilder.DropColumn(
                name: "RoundIsInProgress",
                table: "NinthPlanetGames");
        }
    }
}
