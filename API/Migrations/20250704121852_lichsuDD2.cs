using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class lichsuDD2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuDiemDanhs_DiemDanhs_IdDiemDanh",
                table: "LichSuDiemDanhs");

            migrationBuilder.DropIndex(
                name: "IX_LichSuDiemDanhs_IdDiemDanh",
                table: "LichSuDiemDanhs");

            migrationBuilder.RenameColumn(
                name: "IdDiemDanh",
                table: "LichSuDiemDanhs",
                newName: "IdSinhVien");

            migrationBuilder.AddColumn<Guid>(
                name: "DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiemDanhs_DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs",
                column: "DiemDanhIdDiemDanh");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuDiemDanhs_DiemDanhs_DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs",
                column: "DiemDanhIdDiemDanh",
                principalTable: "DiemDanhs",
                principalColumn: "IdDiemDanh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuDiemDanhs_DiemDanhs_DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs");

            migrationBuilder.DropIndex(
                name: "IX_LichSuDiemDanhs_DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs");

            migrationBuilder.DropColumn(
                name: "DiemDanhIdDiemDanh",
                table: "LichSuDiemDanhs");

            migrationBuilder.RenameColumn(
                name: "IdSinhVien",
                table: "LichSuDiemDanhs",
                newName: "IdDiemDanh");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiemDanhs_IdDiemDanh",
                table: "LichSuDiemDanhs",
                column: "IdDiemDanh");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuDiemDanhs_DiemDanhs_IdDiemDanh",
                table: "LichSuDiemDanhs",
                column: "IdDiemDanh",
                principalTable: "DiemDanhs",
                principalColumn: "IdDiemDanh",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
