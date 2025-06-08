using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class demo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdIP",
                table: "CoSos");

            migrationBuilder.AddColumn<Guid>(
                name: "IdCoSo",
                table: "IPs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCoSo",
                table: "IPs");

            migrationBuilder.AddColumn<Guid>(
                name: "IdIP",
                table: "CoSos",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
