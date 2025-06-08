using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class kaka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoSos_CaHocs_CaHocIdCaHoc",
                table: "CoSos");

            migrationBuilder.DropIndex(
                name: "IX_CoSos_CaHocIdCaHoc",
                table: "CoSos");

            migrationBuilder.DropColumn(
                name: "CaHocIdCaHoc",
                table: "CoSos");

            migrationBuilder.DropColumn(
                name: "IdCaHoc",
                table: "CoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSoId",
                table: "CaHocs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaHocs_CoSoId",
                table: "CaHocs",
                column: "CoSoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaHocs_CoSos_CoSoId",
                table: "CaHocs",
                column: "CoSoId",
                principalTable: "CoSos",
                principalColumn: "IdCoSo",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaHocs_CoSos_CoSoId",
                table: "CaHocs");

            migrationBuilder.DropIndex(
                name: "IX_CaHocs_CoSoId",
                table: "CaHocs");

            migrationBuilder.DropColumn(
                name: "CoSoId",
                table: "CaHocs");

            migrationBuilder.AddColumn<Guid>(
                name: "CaHocIdCaHoc",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdCaHoc",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoSos_CaHocIdCaHoc",
                table: "CoSos",
                column: "CaHocIdCaHoc");

            migrationBuilder.AddForeignKey(
                name: "FK_CoSos_CaHocs_CaHocIdCaHoc",
                table: "CoSos",
                column: "CaHocIdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc");
        }
    }
}
