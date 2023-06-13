using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarOfMinds.Context.Migrations
{
    /// <inheritdoc />
    public partial class playerAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerAddress",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerAddress",
                table: "Players");
        }
    }
}
