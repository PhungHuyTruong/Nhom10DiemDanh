using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _566 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs");

            migrationBuilder.AlterColumn<Guid>(
                name: "DuAnIdDuAn",
                table: "NhomXuongs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs",
                column: "DuAnIdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs");

            migrationBuilder.AlterColumn<Guid>(
                name: "DuAnIdDuAn",
                table: "NhomXuongs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs",
                column: "DuAnIdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
