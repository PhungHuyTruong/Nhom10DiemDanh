using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class hahaha : Migration
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

            migrationBuilder.DropColumn(
                name: "IdDiaDiem",
                table: "CoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "IdCoSo",
                table: "DiaDiems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "TenCoSo",
                table: "CoSos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SDT",
                table: "CoSos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaCoSo",
                table: "CoSos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CoSos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "CoSos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DiaDiems_IdCoSo",
                table: "DiaDiems",
                column: "IdCoSo");

            migrationBuilder.CreateIndex(
                name: "IX_CoSos_IdCaHoc",
                table: "CoSos",
                column: "IdCaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_CoSos_IdIP",
                table: "CoSos",
                column: "IdIP");

            migrationBuilder.AddForeignKey(
                name: "FK_CoSos_CaHocs_IdCaHoc",
                table: "CoSos",
                column: "IdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc");

            migrationBuilder.AddForeignKey(
                name: "FK_CoSos_IPs_IdIP",
                table: "CoSos",
                column: "IdIP",
                principalTable: "IPs",
                principalColumn: "IdIP");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaDiems_CoSos_IdCoSo",
                table: "DiaDiems",
                column: "IdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoSos_CaHocs_IdCaHoc",
                table: "CoSos");

            migrationBuilder.DropForeignKey(
                name: "FK_CoSos_IPs_IdIP",
                table: "CoSos");

            migrationBuilder.DropForeignKey(
                name: "FK_DiaDiems_CoSos_IdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropIndex(
                name: "IX_DiaDiems_IdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropIndex(
                name: "IX_CoSos_IdCaHoc",
                table: "CoSos");

            migrationBuilder.DropIndex(
                name: "IX_CoSos_IdIP",
                table: "CoSos");

            migrationBuilder.DropColumn(
                name: "IdCoSo",
                table: "DiaDiems");

            migrationBuilder.AlterColumn<string>(
                name: "TenCoSo",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SDT",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "MaCoSo",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "DiaDiemIdDiaDiem",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdDiaDiem",
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
