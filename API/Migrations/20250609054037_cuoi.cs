using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class cuoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_CoSos_IdCoSo",
                table: "PhuTrachXuongs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCoSo",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "IdVaiTro",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PhuTrachXuongs_VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                column: "VaiTroIdVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA");

            migrationBuilder.AddForeignKey(
                name: "FK_PhuTrachXuongs_CoSos_IdCoSo",
                table: "PhuTrachXuongs",
                column: "IdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_VaiTroIdVaiTro",
                table: "PhuTrachXuongs",
                column: "VaiTroIdVaiTro",
                principalTable: "VaiTros",
                principalColumn: "IdVaiTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns");

            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_CoSos_IdCoSo",
                table: "PhuTrachXuongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PhuTrachXuongs_VaiTros_VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropIndex(
                name: "IX_PhuTrachXuongs_VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropColumn(
                name: "IdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.DropColumn(
                name: "VaiTroIdVaiTro",
                table: "PhuTrachXuongs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCoSo",
                table: "PhuTrachXuongs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CapDoDuAnIdCDDA",
                table: "DuAns",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_CapDoDuAns_CapDoDuAnIdCDDA",
                table: "DuAns",
                column: "CapDoDuAnIdCDDA",
                principalTable: "CapDoDuAns",
                principalColumn: "IdCDDA",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhuTrachXuongs_CoSos_IdCoSo",
                table: "PhuTrachXuongs",
                column: "IdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
