using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class PackagetoRquestRelationHandleOldNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
