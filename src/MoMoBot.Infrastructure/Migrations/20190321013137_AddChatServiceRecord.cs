using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddChatServiceRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ServiceRecords",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "ServiceRecords",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiceRecordId",
                table: "Chats",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRecords_ChatId",
                table: "ServiceRecords",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ServiceRecordId",
                table: "Chats",
                column: "ServiceRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ServiceRecords_ServiceRecordId",
                table: "Chats",
                column: "ServiceRecordId",
                principalTable: "ServiceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Chats_ChatId",
                table: "ServiceRecords",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ServiceRecords_ServiceRecordId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Chats_ChatId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRecords_ChatId",
                table: "ServiceRecords");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ServiceRecordId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ServiceRecords");

            migrationBuilder.DropColumn(
                name: "ServiceRecordId",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ServiceRecords",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);
        }
    }
}
