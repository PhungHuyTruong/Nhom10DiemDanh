using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _7675 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeHoachs_DuAns_DuAnIdDuAn",
                table: "KeHoachs");

            migrationBuilder.DropIndex(
                name: "IX_KeHoachs_DuAnIdDuAn",
                table: "KeHoachs");

            migrationBuilder.DropColumn(
                name: "DuAnIdDuAn",
                table: "KeHoachs");

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachs_IdDuAn",
                table: "KeHoachs",
                column: "IdDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_KeHoachs_DuAns_IdDuAn",
                table: "KeHoachs",
                column: "IdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KeHoachs_DuAns_IdDuAn",
                table: "KeHoachs");

            migrationBuilder.DropIndex(
                name: "IX_KeHoachs_IdDuAn",
                table: "KeHoachs");

            migrationBuilder.AddColumn<Guid>(
                name: "DuAnIdDuAn",
                table: "KeHoachs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachs_DuAnIdDuAn",
                table: "KeHoachs",
                column: "DuAnIdDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_KeHoachs_DuAns_DuAnIdDuAn",
                table: "KeHoachs",
                column: "DuAnIdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
