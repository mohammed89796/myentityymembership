using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchAdminConfirmAndRejectionNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchAdminConfirmationNotes",
                table: "PaymentBatches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BranchAdminConfirmationUrl",
                table: "PaymentBatches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BranchAdminReason",
                table: "PaymentBatches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BranchAdminRejectionUrl",
                table: "PaymentBatches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchAdminConfirmationNotes",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "BranchAdminConfirmationUrl",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "BranchAdminReason",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "BranchAdminRejectionUrl",
                table: "PaymentBatches");
        }
    }
}
