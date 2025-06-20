using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _65 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNhanViens_PhuTrachXuongs_PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNhanViens_VaiTros_IdVaiTro",
                table: "VaiTroNhanViens");

            migrationBuilder.DropIndex(
                name: "IX_VaiTroNhanViens_PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens");

            migrationBuilder.DropIndex(
                name: "IX_PhuTrachXuongs_IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropColumn(
                name: "PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens");

            migrationBuilder.DropColumn(
                name: "IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTroNhanViens_IdNhanVien",
                table: "VaiTroNhanViens",
                column: "IdNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNhanViens_PhuTrachXuongs_IdNhanVien",
                table: "VaiTroNhanViens",
                column: "IdNhanVien",
                principalTable: "PhuTrachXuongs",
                principalColumn: "IdNhanVien",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNhanViens_VaiTros_IdVaiTro",
                table: "VaiTroNhanViens",
                column: "IdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNhanViens_PhuTrachXuongs_IdNhanVien",
                table: "VaiTroNhanViens");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNhanViens_VaiTros_IdVaiTro",
                table: "VaiTroNhanViens");

            migrationBuilder.DropIndex(
                name: "IX_VaiTroNhanViens_IdNhanVien",
                table: "VaiTroNhanViens");

            migrationBuilder.AddColumn<Guid>(
                name: "PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdVaiTro",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTroNhanViens_PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens",
                column: "PhuTrachXuongIdNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PhuTrachXuongs_IdVaiTro",
                table: "PhuTrachXuongs",
                column: "IdVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_IdVaiTro",
                table: "PhuTrachXuongs",
                column: "IdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNhanViens_PhuTrachXuongs_PhuTrachXuongIdNhanVien",
                table: "VaiTroNhanViens",
                column: "PhuTrachXuongIdNhanVien",
                principalTable: "PhuTrachXuongs",
                principalColumn: "IdNhanVien",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNhanViens_VaiTros_IdVaiTro",
                table: "VaiTroNhanViens",
                column: "IdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro");
        }
    }
}
