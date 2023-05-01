using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarOfMinds.Context.Migrations
{
    /// <inheritdoc />
    public partial class gamePlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Games_GamesGameID",
                table: "GamePlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Players_PlayersPlayerID",
                table: "GamePlayer");

            migrationBuilder.DropTable(
                name: "GamePlayers");

            migrationBuilder.RenameColumn(
                name: "PlayersPlayerID",
                table: "GamePlayer",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "GamesGameID",
                table: "GamePlayer",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlayer_PlayersPlayerID",
                table: "GamePlayer",
                newName: "IX_GamePlayer_GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Games_GameId",
                table: "GamePlayer",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Players_PlayerId",
                table: "GamePlayer",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Games_GameId",
                table: "GamePlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Players_PlayerId",
                table: "GamePlayer");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "GamePlayer",
                newName: "PlayersPlayerID");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "GamePlayer",
                newName: "GamesGameID");

            migrationBuilder.RenameIndex(
                name: "IX_GamePlayer_GameId",
                table: "GamePlayer",
                newName: "IX_GamePlayer_PlayersPlayerID");

            migrationBuilder.CreateTable(
                name: "GamePlayers",
                columns: table => new
                {
                    GamePlayerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<int>(type: "int", nullable: false),
                    PlayerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayers", x => x.GamePlayerID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Games_GamesGameID",
                table: "GamePlayer",
                column: "GamesGameID",
                principalTable: "Games",
                principalColumn: "GameID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Players_PlayersPlayerID",
                table: "GamePlayer",
                column: "PlayersPlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
