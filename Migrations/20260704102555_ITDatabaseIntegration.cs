using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenAPI.Migrations
{
    /// <inheritdoc />
    public partial class ITDatabaseIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Employees_EmployeeID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Employees_EmployeeID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_TodaysSpecials_Employees_CreatedByEmployeeID",
                table: "TodaysSpecials");

            migrationBuilder.DropIndex(
                name: "IX_TodaysSpecials_CreatedByEmployeeID",
                table: "TodaysSpecials");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByID",
                table: "TodaysSpecials",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NewUserID",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NewUserID",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NewUsers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    CreatedByNewUserID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedByEmployeeID = table.Column<int>(type: "int", nullable: true),
                    LockedByNewUserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NewUsers_NewUsers_CreatedByNewUserID",
                        column: x => x.CreatedByNewUserID,
                        principalTable: "NewUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewUsers_NewUsers_LockedByNewUserID",
                        column: x => x.LockedByNewUserID,
                        principalTable: "NewUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodaysSpecials_CreatedByID",
                table: "TodaysSpecials",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NewUserID",
                table: "Notifications",
                column: "NewUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_NewUserID",
                table: "Bookings",
                column: "NewUserID");

            migrationBuilder.CreateIndex(
                name: "IX_NewUsers_CreatedByNewUserID",
                table: "NewUsers",
                column: "CreatedByNewUserID");

            migrationBuilder.CreateIndex(
                name: "IX_NewUsers_LockedByNewUserID",
                table: "NewUsers",
                column: "LockedByNewUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_NewUsers_NewUserID",
                table: "Bookings",
                column: "NewUserID",
                principalTable: "NewUsers",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NewUsers_NewUserID",
                table: "Notifications",
                column: "NewUserID",
                principalTable: "NewUsers",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_NewUsers_NewUserID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NewUsers_NewUserID",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NewUsers");

            migrationBuilder.DropIndex(
                name: "IX_TodaysSpecials_CreatedByID",
                table: "TodaysSpecials");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NewUserID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_NewUserID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedByID",
                table: "TodaysSpecials");

            migrationBuilder.DropColumn(
                name: "NewUserID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NewUserID",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodaysSpecials_CreatedByEmployeeID",
                table: "TodaysSpecials",
                column: "CreatedByEmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Employees_EmployeeID",
                table: "Bookings",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Employees_EmployeeID",
                table: "Notifications",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodaysSpecials_Employees_CreatedByEmployeeID",
                table: "TodaysSpecials",
                column: "CreatedByEmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
