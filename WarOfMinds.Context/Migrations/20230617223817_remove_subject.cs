using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarOfMinds.Context.Migrations
{
    /// <inheritdoc />
    public partial class removesubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Subjects_SubjectID",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Games_SubjectID",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SubjectID",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "SubjectID",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_SubjectID",
                table: "Games",
                column: "SubjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Subjects_SubjectID",
                table: "Games",
                column: "SubjectID",
                principalTable: "Subjects",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
