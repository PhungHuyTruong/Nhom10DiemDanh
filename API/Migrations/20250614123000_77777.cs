using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _77777 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoSos_DiaDiems_DiaDiemIdDiaDiem",
                table: "CoSos");

            migrationBuilder.DropIndex(
                name: "IX_CoSos_DiaDiemIdDiaDiem",
                table: "CoSos");

            migrationBuilder.DropColumn(
                name: "DiaDiemIdDiaDiem",
                table: "CoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSosIdCoSo",
                table: "DiaDiems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdCoSo",
                table: "DiaDiems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DiaDiems_CoSosIdCoSo",
                table: "DiaDiems",
                column: "CoSosIdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaDiems_CoSos_CoSosIdCoSo",
                table: "DiaDiems",
                column: "CoSosIdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiaDiems_CoSos_CoSosIdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropIndex(
                name: "IX_DiaDiems_CoSosIdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropColumn(
                name: "CoSosIdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropColumn(
                name: "IdCoSo",
                table: "DiaDiems");

            migrationBuilder.AddColumn<Guid>(
                name: "DiaDiemIdDiaDiem",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoSos_DiaDiemIdDiaDiem",
                table: "CoSos",
                column: "DiaDiemIdDiaDiem");

            migrationBuilder.AddForeignKey(
                name: "FK_CoSos_DiaDiems_DiaDiemIdDiaDiem",
                table: "CoSos",
                column: "DiaDiemIdDiaDiem",
                principalTable: "DiaDiems",
                principalColumn: "IdDiaDiem");
        }
    }
}
