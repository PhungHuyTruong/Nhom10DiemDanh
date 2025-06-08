using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class demo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoSos_IPs_IPIdIP",
                table: "CoSos");

            migrationBuilder.DropIndex(
                name: "IX_CoSos_IPIdIP",
                table: "CoSos");

            migrationBuilder.DropColumn(
                name: "IPIdIP",
                table: "CoSos");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "IPIdIP",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoSos_IPIdIP",
                table: "CoSos",
                column: "IPIdIP");

            migrationBuilder.AddForeignKey(
                name: "FK_CoSos_IPs_IPIdIP",
                table: "CoSos",
                column: "IPIdIP",
                principalTable: "IPs",
                principalColumn: "IdIP");
        }
    }
}
