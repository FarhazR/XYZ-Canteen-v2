using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAddOnsAndBookingAddOns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingGroupID",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AddOns",
                columns: table => new
                {
                    AddOnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddOns", x => x.AddOnID);
                });

            migrationBuilder.CreateTable(
                name: "BookingAddOns",
                columns: table => new
                {
                    BookingAddOnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingID = table.Column<int>(type: "int", nullable: false),
                    AddOnID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CostPerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAddOns", x => x.BookingAddOnID);
                    table.ForeignKey(
                        name: "FK_BookingAddOns_AddOns_AddOnID",
                        column: x => x.AddOnID,
                        principalTable: "AddOns",
                        principalColumn: "AddOnID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingAddOns_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "BookingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddOns_AddOnID",
                table: "BookingAddOns",
                column: "AddOnID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddOns_BookingID",
                table: "BookingAddOns",
                column: "BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingAddOns");

            migrationBuilder.DropTable(
                name: "AddOns");

            migrationBuilder.DropColumn(
                name: "BookingGroupID",
                table: "Bookings");
        }
    }
}
