using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class SeedingApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6dc6528a-b280-4770-9eae-82671ee81ef7", 0, "99d2bbc6-bc54-4248-a172-a77de3ae4430", "Mohamed@E_Commerce.com", false, "Mohamed", "Hassanein", false, null, "Mohamed@E_Commerce.com", "MOHAMED@E_COMMERCE.COM", "AQAAAAIAAYagAAAAEOh+NqeiN546A10OVMz8PP5ov+8LHGp1j/4ISOir43labqJrQkQdZTekO3HJumb4QA==", null, false, "55BF92C9EF0249CDA210D85D1A851BC9", false, "Mohamed@E_Commerce.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7");
        }
    }
}
