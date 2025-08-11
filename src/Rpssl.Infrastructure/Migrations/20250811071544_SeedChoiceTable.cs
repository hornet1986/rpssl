using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rpssl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedChoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Choices",
                columns: ["Id", "Name"],
                values: new object[,]
                {
                    { 1, "Rock" },
                    { 2, "Paper" },
                    { 3, "Scissors" },
                    { 4, "Lizard" },
                    { 5, "Spock" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Choices",
                keyColumn: "Id",
                keyValues: [1, 2, 3, 4, 5]);
        }
    }
}
