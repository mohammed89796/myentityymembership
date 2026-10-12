using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class packageMembershipConfaguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Memberships_PackageId",
                table: "Memberships",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Packages_PackageId",
                table: "Memberships",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Packages_PackageId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_PackageId",
                table: "Memberships");
        }
    }
}
