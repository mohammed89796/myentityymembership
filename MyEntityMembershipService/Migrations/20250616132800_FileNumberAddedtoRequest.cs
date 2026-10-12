using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class FileNumberAddedtoRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileNumber",
                table: "MembershipRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_TenantId",
                table: "Batches",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Tenants_TenantId",
                table: "Batches",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Tenants_TenantId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_TenantId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "FileNumber",
                table: "MembershipRequests");
        }
    }
}
