using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class addFromTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileNumber",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromTenantId",
                table: "MembershipRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequests_FromTenantId",
                table: "MembershipRequests",
                column: "FromTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequests_Tenants_FromTenantId",
                table: "MembershipRequests",
                column: "FromTenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequests_Tenants_FromTenantId",
                table: "MembershipRequests");

            migrationBuilder.DropIndex(
                name: "IX_MembershipRequests_FromTenantId",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "FileNumber",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "FromTenantId",
                table: "MembershipRequests");
        }
    }
}
