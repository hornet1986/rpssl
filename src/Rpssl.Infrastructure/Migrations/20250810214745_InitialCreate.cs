using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rpssl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PlayerChoiceId = table.Column<int>(type: "int", nullable: false),
                    ComputerChoiceId = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameResults_Choices_ComputerChoiceId",
                        column: x => x.ComputerChoiceId,
                        principalTable: "Choices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameResults_Choices_PlayerChoiceId",
                        column: x => x.PlayerChoiceId,
                        principalTable: "Choices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_Name",
                table: "Choices",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_ComputerChoiceId",
                table: "GameResults",
                column: "ComputerChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_Outcome",
                table: "GameResults",
                column: "Outcome");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_PlayedAt",
                table: "GameResults",
                column: "PlayedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_PlayerChoiceId",
                table: "GameResults",
                column: "PlayerChoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameResults");

            migrationBuilder.DropTable(
                name: "Choices");
        }
    }
}
