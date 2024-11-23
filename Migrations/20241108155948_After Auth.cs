using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrental.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AfterAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookingConfirmations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BookingConfirmations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
