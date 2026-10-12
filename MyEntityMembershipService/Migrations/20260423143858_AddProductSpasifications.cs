using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSpasifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxEvents",
                table: "PackageProducts",
                newName: "SessionInterval");

            migrationBuilder.AddColumn<int>(
                name: "PerInterval",
                table: "PackageProducts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerInterval",
                table: "PackageProducts");

            migrationBuilder.RenameColumn(
                name: "SessionInterval",
                table: "PackageProducts",
                newName: "MaxEvents");
        }
    }
}
