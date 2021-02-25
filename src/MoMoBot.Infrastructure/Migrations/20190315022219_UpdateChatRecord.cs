using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class UpdateChatRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Content = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Who = table.Column<int>(nullable: false),
                    Unread = table.Column<bool>(nullable: false),
                    Data = table.Column<string>(maxLength: 1024, nullable: true),
                    ChatId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRecords", x => x.Id);
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRecords");
        }
    }
}
