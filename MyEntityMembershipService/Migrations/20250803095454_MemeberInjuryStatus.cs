using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class MemeberInjuryStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InjuryStatus",
                table: "Memberships",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InjuryStatus",
                table: "Memberships");
        }
    }
}
