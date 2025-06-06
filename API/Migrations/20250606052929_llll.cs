using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class llll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdKHNX",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCaHoc",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs",
                column: "IdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc");

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs",
                column: "IdKHNX",
                principalTable: "KeHoachNhomXuongs",
                principalColumn: "IdKHNX");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdKHNX",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCaHoc",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs",
                column: "IdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs",
                column: "IdKHNX",
                principalTable: "KeHoachNhomXuongs",
                principalColumn: "IdKHNX",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
