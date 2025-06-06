using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeHoachThoiGian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NgayDienRa",
                table: "KeHoachs",
                newName: "ThoiGianKetThuc");

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianBatDau",
                table: "KeHoachs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianBatDau",
                table: "KeHoachs");

            migrationBuilder.RenameColumn(
                name: "ThoiGianKetThuc",
                table: "KeHoachs",
                newName: "NgayDienRa");
        }
    }
}
