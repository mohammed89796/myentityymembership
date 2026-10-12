using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class MembershipRequestAccessNullvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewFacility_FacilityId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewVenue_VenueId",
                table: "MembershipRequestAccess");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "MembershipRequestAccess",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "MembershipRequestAccess",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequestAccess_NewFacility_FacilityId",
                table: "MembershipRequestAccess",
                column: "FacilityId",
                principalTable: "NewFacility",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequestAccess_NewVenue_VenueId",
                table: "MembershipRequestAccess",
                column: "VenueId",
                principalTable: "NewVenue",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewFacility_FacilityId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewVenue_VenueId",
                table: "MembershipRequestAccess");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "MembershipRequestAccess",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "MembershipRequestAccess",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequestAccess_NewFacility_FacilityId",
                table: "MembershipRequestAccess",
                column: "FacilityId",
                principalTable: "NewFacility",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequestAccess_NewVenue_VenueId",
                table: "MembershipRequestAccess",
                column: "VenueId",
                principalTable: "NewVenue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
