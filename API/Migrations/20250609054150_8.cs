using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class _8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropIndex(
                name: "IX_PhuTrachXuongs_VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropColumn(
                name: "VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropIndex(
                name: "IX_PhuTrachXuongs_IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.AddColumn<Guid>(
                name: "VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhuTrachXuongs_VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                column: "VaiTroIdVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                column: "VaiTroIdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro");
        }
    }
}
