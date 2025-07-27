using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class lklk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoMonCoSos_QuanLyBoMons_IdBoMon",
                table: "BoMonCoSos");

            migrationBuilder.DropForeignKey(
                name: "FK_DiaDiems_CoSos_CoSosIdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHocs_HocKys_HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropIndex(
                name: "IX_LichHocs_HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropColumn(
                name: "HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropColumn(
                name: "TenBoMon",
                table: "KeHoachs");

            migrationBuilder.DropColumn(
                name: "TenCapDoDuAn",
                table: "KeHoachs");

            migrationBuilder.DropColumn(
                name: "TenDuAn",
                table: "KeHoachs");

            migrationBuilder.RenameColumn(
                name: "CoSosIdCoSo",
                table: "DiaDiems",
                newName: "CoSoIdCoSo");

            migrationBuilder.RenameIndex(
                name: "IX_DiaDiems_CoSosIdCoSo",
                table: "DiaDiems",
                newName: "IX_DiaDiems_CoSoIdCoSo");

            migrationBuilder.AlterColumn<string>(
                name: "TenVaiTro",
                table: "VaiTros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdVaiTro",
                table: "VaiTroNhanViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdNhanVien",
                table: "VaiTroNhanViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenBoMon",
                table: "QuanLyBoMons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaBoMon",
                table: "QuanLyBoMons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CoSoHoatDong",
                table: "QuanLyBoMons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDungBuoiHoc",
                table: "LichSuDiemDanhs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HinhThuc",
                table: "LichSuDiemDanhs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichSuDiemDanhs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiem",
                table: "LichSuDiemDanhs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HTGiangDay",
                table: "LichGiangDays",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdKHNX",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCaHoc",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiemDanhTre",
                table: "KHNXCaHocs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Buoi",
                table: "KHNXCaHocs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KieuIP",
                table: "IPs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IP_DaiIP",
                table: "IPs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ImportHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ImportHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ImportHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImportedBy",
                table: "ImportHistory",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ImportHistory",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TenCoSo",
                table: "CoSos",
                type: "nvarchar(255)",
                maxLength: 255,
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
                type: "nvarchar(255)",
                maxLength: 255,
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

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCoSo",
                table: "BoMonCoSos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdBoMon",
                table: "BoMonCoSos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BoMonCoSos_QuanLyBoMons_IdBoMon",
                table: "BoMonCoSos",
                column: "IdBoMon",
                principalTable: "QuanLyBoMons",
                principalColumn: "IDBoMon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiaDiems_CoSos_CoSoIdCoSo",
                table: "DiaDiems",
                column: "CoSoIdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs",
                column: "IdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs",
                column: "IdKHNX",
                principalTable: "KeHoachNhomXuongs",
                principalColumn: "IdKHNX",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichHocs_HocKys_IDHocKy",
                table: "LichHocs",
                column: "IDHocKy",
                principalTable: "HocKys",
                principalColumn: "IdHocKy",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoMonCoSos_QuanLyBoMons_IdBoMon",
                table: "BoMonCoSos");

            migrationBuilder.DropForeignKey(
                name: "FK_DiaDiems_CoSos_CoSoIdCoSo",
                table: "DiaDiems");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHocs_HocKys_IDHocKy",
                table: "LichHocs");

            migrationBuilder.RenameColumn(
                name: "CoSoIdCoSo",
                table: "DiaDiems",
                newName: "CoSosIdCoSo");

            migrationBuilder.RenameIndex(
                name: "IX_DiaDiems_CoSoIdCoSo",
                table: "DiaDiems",
                newName: "IX_DiaDiems_CoSosIdCoSo");

            migrationBuilder.AlterColumn<string>(
                name: "TenVaiTro",
                table: "VaiTros",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdVaiTro",
                table: "VaiTroNhanViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdNhanVien",
                table: "VaiTroNhanViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TenBoMon",
                table: "QuanLyBoMons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MaBoMon",
                table: "QuanLyBoMons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CoSoHoatDong",
                table: "QuanLyBoMons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDungBuoiHoc",
                table: "LichSuDiemDanhs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "HinhThuc",
                table: "LichSuDiemDanhs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "LichSuDiemDanhs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "DiaDiem",
                table: "LichSuDiemDanhs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<Guid>(
                name: "HocKyIdHocKy",
                table: "LichHocs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "HTGiangDay",
                table: "LichGiangDays",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdKHNX",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCaHoc",
                table: "KHNXCaHocs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DiemDanhTre",
                table: "KHNXCaHocs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Buoi",
                table: "KHNXCaHocs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "TenBoMon",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenCapDoDuAn",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenDuAn",
                table: "KeHoachs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KieuIP",
                table: "IPs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "IP_DaiIP",
                table: "IPs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ImportHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ImportHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ImportHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ImportedBy",
                table: "ImportHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "ImportHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "TenCoSo",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

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
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "CoSos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCoSo",
                table: "BoMonCoSos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdBoMon",
                table: "BoMonCoSos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_LichHocs_HocKyIdHocKy",
                table: "LichHocs",
                column: "HocKyIdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_BoMonCoSos_QuanLyBoMons_IdBoMon",
                table: "BoMonCoSos",
                column: "IdBoMon",
                principalTable: "QuanLyBoMons",
                principalColumn: "IDBoMon");

            migrationBuilder.AddForeignKey(
                name: "FK_DiaDiems_CoSos_CoSosIdCoSo",
                table: "DiaDiems",
                column: "CoSosIdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_CaHocs_IdCaHoc",
                table: "KHNXCaHocs",
                column: "IdCaHoc",
                principalTable: "CaHocs",
                principalColumn: "IdCaHoc");

            migrationBuilder.AddForeignKey(
                name: "FK_KHNXCaHocs_KeHoachNhomXuongs_IdKHNX",
                table: "KHNXCaHocs",
                column: "IdKHNX",
                principalTable: "KeHoachNhomXuongs",
                principalColumn: "IdKHNX");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHocs_HocKys_HocKyIdHocKy",
                table: "LichHocs",
                column: "HocKyIdHocKy",
                principalTable: "HocKys",
                principalColumn: "IdHocKy",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
