using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class HandleProductScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Packages",
                newName: "MembershipMode");

            migrationBuilder.AddColumn<int>(
                name: "PackageScope",
                table: "Packages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageScope",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "MembershipMode",
                table: "Packages",
                newName: "Features");
        }
    }
}
