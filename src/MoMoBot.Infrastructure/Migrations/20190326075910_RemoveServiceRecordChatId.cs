using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class RemoveServiceRecordChatId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Chats_ChatId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRecords_ChatId",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ServiceRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "ServiceRecords",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_ChatId",
                table: "ServiceRecords",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Chats_ChatId",
                table: "ServiceRecords",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
