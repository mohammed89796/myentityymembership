using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class MembershipAccessesmaptonewtables : Migration
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

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_NewFacility_FacilityId",
                table: "MembershipAccesses",
                column: "FacilityId",
                principalTable: "NewFacility",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipAccesses_NewVenue_VenueId",
                table: "MembershipAccesses",
                column: "VenueId",
                principalTable: "NewVenue",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_NewFacility_FacilityId",
                table: "MembershipAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipAccesses_NewVenue_VenueId",
                table: "MembershipAccesses");

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
    }
}
