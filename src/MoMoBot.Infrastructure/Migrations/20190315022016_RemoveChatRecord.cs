using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class RemoveChatRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRecords",
                columns: table => new
                {
                    guid = table.Column<Guid>(nullable: false),
                    ChatId = table.Column<long>(nullable: true),
                    Content = table.Column<string>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 200, nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    Who = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRecords", x => x.guid);
                    table.ForeignKey(
                        name: "FK_ChatRecords_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRecords_ChatId",
                table: "ChatRecords",
                column: "ChatId");
        }
    }
}
