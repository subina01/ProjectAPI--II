using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrental.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class F : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnLocation",
                table: "ReturnConfirmations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReturnConfirmations");

            migrationBuilder.AddColumn<string>(
                name: "ReturnLocation",
                table: "Returns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnLocation",
                table: "Returns");

            migrationBuilder.AddColumn<string>(
                name: "ReturnLocation",
                table: "ReturnConfirmations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReturnConfirmations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
