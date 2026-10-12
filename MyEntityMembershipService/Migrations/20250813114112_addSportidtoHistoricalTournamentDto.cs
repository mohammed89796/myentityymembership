using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class addSportidtoHistoricalTournamentDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenueOperatingHours_NewVenue_VenueId",
                table: "VenueOperatingHours");

            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "HistoricalTournament",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VenueOperatingHours_NewVenueId",
                table: "VenueOperatingHours",
                column: "NewVenueId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VenueOperatingHours_NewVenue_NewVenueId",
                table: "VenueOperatingHours",
                column: "NewVenueId",
                principalTable: "NewVenue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenueOperatingHours_NewVenue_NewVenueId",
                table: "VenueOperatingHours");

            migrationBuilder.DropIndex(
                name: "IX_VenueOperatingHours_NewVenueId",
                table: "VenueOperatingHours");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "HistoricalTournament");

            migrationBuilder.AddForeignKey(
                name: "FK_VenueOperatingHours_NewVenue_VenueId",
                table: "VenueOperatingHours",
                column: "VenueId",
                principalTable: "NewVenue",
                principalColumn: "Id");
        }
    }
}
