using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TgBotForMedUniversity.Migrations
{
    /// <inheritdoc />
    public partial class ChangedQuestStateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedOptions",
                table: "QuestionStates",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedOptions",
                table: "QuestionStates");
        }
    }
}
