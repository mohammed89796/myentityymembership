using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class updatemembershipandrequstdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateOfAcquisition",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfExpiry",
                table: "Memberships",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfIssuance",
                table: "Memberships",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeNumber",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MilitaryStatus",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SchoolOrWorkEntity",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfExpiry",
                table: "MembershipRequests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfIssuance",
                table: "MembershipRequests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeNumber",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LoanRequestDocument",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MilitaryStatus",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SchoolOrWorkEntity",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TransferRequestDocument",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfAcquisition",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DateOfExpiry",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DateOfIssuance",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "HomeNumber",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "MilitaryStatus",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "SchoolOrWorkEntity",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DateOfExpiry",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "DateOfIssuance",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "HomeNumber",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "LoanRequestDocument",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "MilitaryStatus",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "SchoolOrWorkEntity",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "TransferRequestDocument",
                table: "MembershipRequests");
        }
    }
}
