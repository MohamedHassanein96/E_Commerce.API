using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnsname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "ReservedStock");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Carts",
                newName: "Quantity");

            migrationBuilder.AddColumn<int>(
                name: "AvailableStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePath",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InvoicePath",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ReservedStock",
                table: "Products",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Carts",
                newName: "Count");
        }
    }
}
