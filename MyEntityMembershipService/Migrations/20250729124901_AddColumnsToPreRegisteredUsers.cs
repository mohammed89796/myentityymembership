using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsToPreRegisteredUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeltName",
                table: "PreRegisteredUsers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "KarateBeltDate",
                table: "PreRegisteredUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RegisterationDate",
                table: "PreRegisteredUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterationSerialNumber",
                table: "PreRegisteredUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeltName",
                table: "PreRegisteredUsers");

            migrationBuilder.DropColumn(
                name: "KarateBeltDate",
                table: "PreRegisteredUsers");

            migrationBuilder.DropColumn(
                name: "RegisterationDate",
                table: "PreRegisteredUsers");

            migrationBuilder.DropColumn(
                name: "RegisterationSerialNumber",
                table: "PreRegisteredUsers");
        }
    }
}
