using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionStatusToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollected",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MealPricing",
                columns: table => new
                {
                    MealPricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPricing", x => x.MealPricingID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPricing");

            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsCollected",
                table: "Bookings");
        }
    }
}
