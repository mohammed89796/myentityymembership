using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class PackageToRequestMappingRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequests_PackageId",
                table: "MembershipRequests",
                column: "PackageId");
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS=0;");


            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS=1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequests_Packages_PackageId",
                table: "MembershipRequests");

            migrationBuilder.DropIndex(
                name: "IX_MembershipRequests_PackageId",
                table: "MembershipRequests");
        }
    }
}
