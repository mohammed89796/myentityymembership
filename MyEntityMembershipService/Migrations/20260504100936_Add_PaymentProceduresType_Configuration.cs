using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class Add_PaymentProceduresType_Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "RenewalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "RemovalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "NewMembershipConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "FreezeConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "CancellationConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentProceduresType",
                table: "AdditionalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "RenewalConfigurations");

            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "RemovalConfigurations");

            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "NewMembershipConfiguration");

            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "FreezeConfigurations");

            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "CancellationConfigurations");

            migrationBuilder.DropColumn(
                name: "PaymentProceduresType",
                table: "AdditionalConfigurations");
        }
    }
}
