using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class BoMonCoSo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoMonCoSos_CoSos_CoSoIdCoSo",
                table: "BoMonCoSos");

            migrationBuilder.DropIndex(
                name: "IX_BoMonCoSos_CoSoIdCoSo",
                table: "BoMonCoSos");

            migrationBuilder.DropColumn(
                name: "CoSoIdCoSo",
                table: "BoMonCoSos");

            migrationBuilder.CreateIndex(
                name: "IX_BoMonCoSos_IdCoSo",
                table: "BoMonCoSos",
                column: "IdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_BoMonCoSos_CoSos_IdCoSo",
                table: "BoMonCoSos",
                column: "IdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoMonCoSos_CoSos_IdCoSo",
                table: "BoMonCoSos");

            migrationBuilder.DropIndex(
                name: "IX_BoMonCoSos_IdCoSo",
                table: "BoMonCoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "CoSoIdCoSo",
                table: "BoMonCoSos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BoMonCoSos_CoSoIdCoSo",
                table: "BoMonCoSos",
                column: "CoSoIdCoSo");

            migrationBuilder.AddForeignKey(
                name: "FK_BoMonCoSos_CoSos_CoSoIdCoSo",
                table: "BoMonCoSos",
                column: "CoSoIdCoSo",
                principalTable: "CoSos",
                principalColumn: "IdCoSo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
