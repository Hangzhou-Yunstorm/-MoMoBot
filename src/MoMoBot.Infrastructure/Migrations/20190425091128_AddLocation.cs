using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Steps",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "X",
                table: "Steps",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Y",
                table: "Steps",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "X",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Steps");
        }
    }
}
