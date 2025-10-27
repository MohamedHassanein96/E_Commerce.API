using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class removeSentimentAndHuggingApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
