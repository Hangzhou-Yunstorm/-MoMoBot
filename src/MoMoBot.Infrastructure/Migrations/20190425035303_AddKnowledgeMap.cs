using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoMoBot.Infrastructure.Migrations
{
    public partial class AddKnowledgeMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnswerType",
                table: "Answers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FlowId",
                table: "Answers",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceUrl",
                table: "Answers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KnowledgeMaps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Identifier = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MapId = table.Column<long>(nullable: false),
                    PrevStep = table.Column<long>(nullable: true),
                    NextStep = table.Column<long>(nullable: true),
                    StepType = table.Column<int>(nullable: false),
                    Question = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    ResultType = table.Column<string>(nullable: true),
                    TriggeredResult = table.Column<string>(nullable: true),
                    Function = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Steps_KnowledgeMaps_MapId",
                        column: x => x.MapId,
                        principalTable: "KnowledgeMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Steps_MapId",
                table: "Steps",
                column: "MapId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "KnowledgeMaps");

            migrationBuilder.DropColumn(
                name: "AnswerType",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "FlowId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ResourceUrl",
                table: "Answers");
        }
    }
}
