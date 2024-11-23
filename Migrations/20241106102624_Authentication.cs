using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrental.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Authentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedByUserId",
                table: "Vehicles",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Returns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReturnConfirmations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BookingConfirmations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Returns");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReturnConfirmations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookingConfirmations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Vehicles",
                newName: "AddedByUserId");
        }
    }
}
