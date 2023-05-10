using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarOfMinds.Context.Migrations
{
    /// <inheritdoc />
    public partial class gameonhold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnHold",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnHold",
                table: "Games");
        }
    }
}
