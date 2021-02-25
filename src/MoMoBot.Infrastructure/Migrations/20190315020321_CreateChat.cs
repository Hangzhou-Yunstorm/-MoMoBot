using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class CreateChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Owner = table.Column<string>(nullable: false),
                    Other = table.Column<string>(nullable: false),
                    DisplayInList = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 200, nullable: true),
                    Online = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatRecords",
                columns: table => new
                {
                    guid = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Who = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 200, nullable: true),
                    ChatId = table.Column<long>(nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRecords");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
