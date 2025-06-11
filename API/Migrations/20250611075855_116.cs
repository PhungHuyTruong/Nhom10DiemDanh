using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _116 : Migration
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

            migrationBuilder.DropColumn(
                name: "IdIP",
                table: "CoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "IdCoSo",
                table: "IPs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.DropColumn(
                name: "IdCoSo",
                table: "IPs");

            migrationBuilder.AddColumn<Guid>(
                name: "IPIdIP",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdIP",
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
