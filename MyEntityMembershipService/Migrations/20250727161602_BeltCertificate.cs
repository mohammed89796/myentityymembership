using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class BeltCertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeltCertificate",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BeltCertificate",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeltCertificate",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "BeltCertificate",
                table: "MembershipRequests");
        }
    }
}
