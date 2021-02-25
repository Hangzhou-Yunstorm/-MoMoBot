using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddMapStepTempId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempId",
                table: "Steps",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempId",
                table: "Steps");
        }
    }
}
