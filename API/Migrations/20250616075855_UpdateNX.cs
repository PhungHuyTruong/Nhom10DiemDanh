using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs");

            migrationBuilder.DropIndex(
                name: "IX_NhomXuongs_DuAnIdDuAn",
                table: "NhomXuongs");

            migrationBuilder.DropColumn(
                name: "DuAnIdDuAn",
                table: "NhomXuongs");

            migrationBuilder.CreateIndex(
                name: "IX_NhomXuongs_IdDuAn",
                table: "NhomXuongs",
                column: "IdDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_NhomXuongs_DuAns_IdDuAn",
                table: "NhomXuongs",
                column: "IdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhomXuongs_DuAns_IdDuAn",
                table: "NhomXuongs");

            migrationBuilder.DropIndex(
                name: "IX_NhomXuongs_IdDuAn",
                table: "NhomXuongs");

            migrationBuilder.AddColumn<Guid>(
                name: "DuAnIdDuAn",
                table: "NhomXuongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhomXuongs_DuAnIdDuAn",
                table: "NhomXuongs",
                column: "DuAnIdDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_NhomXuongs_DuAns_DuAnIdDuAn",
                table: "NhomXuongs",
                column: "DuAnIdDuAn",
                principalTable: "DuAns",
                principalColumn: "IdDuAn");
        }
    }
}
