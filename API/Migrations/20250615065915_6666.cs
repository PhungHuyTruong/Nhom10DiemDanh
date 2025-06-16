using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _6666 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenBoMon",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenCapDoDuAn",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenDuAn",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenBoMon",
                table: "KeHoachs");

            migrationBuilder.DropColumn(
                name: "TenCapDoDuAn",
                table: "KeHoachs");

            migrationBuilder.DropColumn(
                name: "TenDuAn",
                table: "KeHoachs");
        }
    }
}
