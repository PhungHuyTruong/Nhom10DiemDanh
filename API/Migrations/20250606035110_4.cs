using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_NhomXuongs_NhomXuongIdNhomXuong",
                table: "SinhViens");

            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_VaiTros_VaiTroIdVaiTro",
                table: "SinhViens");

            migrationBuilder.AlterColumn<Guid>(
                name: "VaiTroIdVaiTro",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "SinhViens",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TenSinhVien",
                table: "SinhViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "NhomXuongIdNhomXuong",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MaSinhVien",
                table: "SinhViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdVaiTro",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdNhomXuong",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SinhViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaHocKy",
                table: "hocKy",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_NhomXuongs_NhomXuongIdNhomXuong",
                table: "SinhViens",
                column: "NhomXuongIdNhomXuong",
                principalTable: "NhomXuongs",
                principalColumn: "IdNhomXuong");

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_VaiTros_VaiTroIdVaiTro",
                table: "SinhViens",
                column: "VaiTroIdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_NhomXuongs_NhomXuongIdNhomXuong",
                table: "SinhViens");

            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_VaiTros_VaiTroIdVaiTro",
                table: "SinhViens");

            migrationBuilder.AlterColumn<Guid>(
                name: "VaiTroIdVaiTro",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "SinhViens",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TenSinhVien",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "NhomXuongIdNhomXuong",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSinhVien",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdVaiTro",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdNhomXuong",
                table: "SinhViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MaHocKy",
                table: "hocKy",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_NhomXuongs_NhomXuongIdNhomXuong",
                table: "SinhViens",
                column: "NhomXuongIdNhomXuong",
                principalTable: "NhomXuongs",
                principalColumn: "IdNhomXuong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_VaiTros_VaiTroIdVaiTro",
                table: "SinhViens",
                column: "VaiTroIdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
