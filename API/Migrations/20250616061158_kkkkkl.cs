using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class kkkkkl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_HocKy_IdHocKy",
                table: "DuAns");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHocs_HocKy_HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HocKy",
                table: "HocKy");

            migrationBuilder.RenameTable(
                name: "HocKy",
                newName: "HocKys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HocKys",
                table: "HocKys",
                column: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_HocKys_IdHocKy",
                table: "DuAns",
                column: "IdHocKy",
                principalTable: "HocKys",
                principalColumn: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHocs_HocKys_HocKyIdHocKy",
                table: "LichHocs",
                column: "HocKyIdHocKy",
                principalTable: "HocKys",
                principalColumn: "IdHocKy",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_HocKys_IdHocKy",
                table: "DuAns");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHocs_HocKys_HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HocKys",
                table: "HocKys");

            migrationBuilder.RenameTable(
                name: "HocKys",
                newName: "HocKy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HocKy",
                table: "HocKy",
                column: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_HocKy_IdHocKy",
                table: "DuAns",
                column: "IdHocKy",
                principalTable: "HocKy",
                principalColumn: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHocs_HocKy_HocKyIdHocKy",
                table: "LichHocs",
                column: "HocKyIdHocKy",
                principalTable: "HocKy",
                principalColumn: "IdHocKy",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
