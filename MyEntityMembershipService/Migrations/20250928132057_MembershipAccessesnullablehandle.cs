using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class MembershipAccessesnullablehandle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_Facilities_FacilityId",
                table: "MembershipAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_Venues_VenueId",
                table: "MembershipAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "MembershipAccesses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "MembershipAccesses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_Facilities_FacilityId",
                table: "MembershipAccesses",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_Venues_VenueId",
                table: "MembershipAccesses",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_Facilities_FacilityId",
                table: "MembershipAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_Venues_VenueId",
                table: "MembershipAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "MembershipAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "MembershipAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_Facilities_FacilityId",
                table: "MembershipAccesses",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_Venues_VenueId",
                table: "MembershipAccesses",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
