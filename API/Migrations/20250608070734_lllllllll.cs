using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class lllllllll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.AlterColumn<Guid>(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.AlterColumn<Guid>(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
