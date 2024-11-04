using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrental.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class authenticated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Returns_BookingId",
                table: "Returns");

            migrationBuilder.AddColumn<int>(
                name: "ReturnConfirmationId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Returns_BookingId",
                table: "Returns",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ReturnConfirmationId",
                table: "Bookings",
                column: "ReturnConfirmationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_ReturnConfirmations_ReturnConfirmationId",
                table: "Bookings",
                column: "ReturnConfirmationId",
                principalTable: "ReturnConfirmations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_ReturnConfirmations_ReturnConfirmationId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Returns_BookingId",
                table: "Returns");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ReturnConfirmationId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ReturnConfirmationId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_BookingId",
                table: "Returns",
                column: "BookingId");
        }
    }
}
