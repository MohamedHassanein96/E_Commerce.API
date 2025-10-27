using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class removeIsoftDeletableFromReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
