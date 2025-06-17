using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class kkkkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DuAns_hocKy_IdHocKy",
                table: "DuAns");

            migrationBuilder.DropForeignKey(
                name: "FK_LichHocs_hocKy_HocKyIdHocKy",
                table: "LichHocs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hocKy",
                table: "hocKy");

            migrationBuilder.RenameTable(
                name: "hocKy",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "hocKy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_hocKy",
                table: "hocKy",
                column: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_DuAns_hocKy_IdHocKy",
                table: "DuAns",
                column: "IdHocKy",
                principalTable: "hocKy",
                principalColumn: "IdHocKy");

            migrationBuilder.AddForeignKey(
                name: "FK_LichHocs_hocKy_HocKyIdHocKy",
                table: "LichHocs",
                column: "HocKyIdHocKy",
                principalTable: "hocKy",
                principalColumn: "IdHocKy",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
