using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddMessageType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChatRecords",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChatRecords");
        }
    }
}
