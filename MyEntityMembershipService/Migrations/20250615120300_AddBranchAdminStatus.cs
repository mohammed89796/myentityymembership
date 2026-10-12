using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchAdminStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchAdminStatus",
                table: "PaymentBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchAdminStatus",
                table: "NewPaymentRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchAdminStatus",
                table: "MembershipRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchAdminStatus",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "BranchAdminStatus",
                table: "NewPaymentRequests");

            migrationBuilder.DropColumn(
                name: "BranchAdminStatus",
                table: "MembershipRequests");
        }
    }
}
