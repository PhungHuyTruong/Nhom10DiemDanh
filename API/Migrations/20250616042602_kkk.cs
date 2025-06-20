using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class kkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.DropIndex(
                name: "IX_DuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.DropColumn(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.AlterColumn<string>(
                name: "TenDuAn",
                table: "DuAns",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "DuAns",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DuAns_IdCDDA",
                table: "DuAns",
                column: "IdCDDA");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_IdCDDA",
                table: "DuAns",
                column: "IdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_IdCDDA",
                table: "DuAns");

            migrationBuilder.DropIndex(
                name: "IX_DuAns_IdCDDA",
                table: "DuAns");

            migrationBuilder.AlterColumn<string>(
                name: "TenDuAn",
                table: "DuAns",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "DuAns",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<Guid>(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA");
        }
    }
}
