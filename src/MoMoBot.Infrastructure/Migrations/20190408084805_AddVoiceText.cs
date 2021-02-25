using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddVoiceText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Voices",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Voices");
        }
    }
}
