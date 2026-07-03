using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMealPricingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "MealPricing",
                newName: "PaneerSurcharge");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseCost",
                table: "MealPricing",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NonVegSurcharge",
                table: "MealPricing",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseCost",
                table: "MealPricing");

            migrationBuilder.DropColumn(
                name: "NonVegSurcharge",
                table: "MealPricing");

            migrationBuilder.RenameColumn(
                name: "PaneerSurcharge",
                table: "MealPricing",
                newName: "Cost");
        }
    }
}
