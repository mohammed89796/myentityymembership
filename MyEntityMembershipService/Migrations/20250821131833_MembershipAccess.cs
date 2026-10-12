using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class MembershipAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackageType",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperPackageId",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperPackageId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsParent",
                table: "MembershipRequests",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_SuperPackageId",
                table: "Packages",
                column: "SuperPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Packages_SuperPackageId",
                table: "Packages",
                column: "SuperPackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Packages_SuperPackageId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_SuperPackageId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PackageType",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SuperPackageId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SuperPackageId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "IsParent",
                table: "MembershipRequests");
        }
    }
}
