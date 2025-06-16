using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPs_CoSos_CoSoIdCoSo",
                table: "IPs");

            migrationBuilder.DropIndex(
                name: "IX_IPs_CoSoIdCoSo",
                table: "IPs");

            migrationBuilder.DropColumn(
                name: "CoSoIdCoSo",
                table: "IPs");

            migrationBuilder.CreateIndex(
                name: "IX_IPs_IdCoSo",
                table: "IPs",
                column: "IdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_IPs_CoSos_IdCoSo",
                table: "IPs",
                column: "IdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IPs_CoSos_IdCoSo",
                table: "IPs");

            migrationBuilder.DropIndex(
                name: "IX_IPs_IdCoSo",
                table: "IPs");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSoIdCoSo",
                table: "IPs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_IPs_CoSoIdCoSo",
                table: "IPs",
                column: "CoSoIdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_IPs_CoSos_CoSoIdCoSo",
                table: "IPs",
                column: "CoSoIdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
