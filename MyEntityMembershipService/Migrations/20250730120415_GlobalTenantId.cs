using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class GlobalTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Venues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueOperatingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueLocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueBookingTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueBookingSlot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "VenueBooking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "UsersChoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "UnitTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TurfTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TenantAreaAdmins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TenantArea",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TenantAdmins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TenantAdminRole",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TeamMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "SubMemberCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "State",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "StadFacilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "SportsAccommodated",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Sports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Shares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Series",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "RequestTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "RequestTypeCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "RenwalOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "RenewalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "RemovalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "PreRegisteredUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "PostTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Polls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "PaymentRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "PaymentBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "PackageConfigurationSections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Organizations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Operations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueUtilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueTurfTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenuePricingRules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueDiscountTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueBookingConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenueAccessibilityOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewVenue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewPaymentRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewMembershipConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewFacilitySports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewFacilityOperatingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewFacilityAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewFacilityAccessibilityOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewFacility",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewCoreUtilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "NewAccessibilityOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "MembershipRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "MembershipRequestComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "MembershipRequestBackgroundJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "MemberType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FreezeConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Forms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FormUserResponse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Fields",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FieldTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FieldResponse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FieldCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FedrationEntityTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FedrationBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityOperatingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityLocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "FacilityAccessibilityOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "EventDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "EntityType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "EntityMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "EntityAdmins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "DropdownValue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "CountryCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Country",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "CoreServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "CoreFacilityServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Configurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "ConfigurationFields",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "City",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Choices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "CancellationConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BranchServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BranchOperatingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BranchLocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BranchImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BookMarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Belts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "AreaPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "AreaConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "Amenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "AdditionalConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "AccessibilityOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "AccessibilityFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueSlots");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueReviews");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueOperatingHours");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueLocation");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueImages");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueBookingTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueBookingSlot");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "VenueBooking");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "UsersChoices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "UnitTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TurfTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TenantAreaAdmins");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TenantArea");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TenantAdmins");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TenantAdminRole");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "SubMemberCategories");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "State");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "StadFacilities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "SportsAccommodated");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Sports");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "RequestTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "RequestTypeCategory");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "RenwalOption");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "RenewalConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "RemovalConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "PreRegisteredUsers");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "PackageConfigurationSections");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueUtilities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueTurfTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenuePricingRules");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueDiscountTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueBookingConfiguration");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueAmenities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenueAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewVenue");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewPaymentRequests");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewMembershipConfiguration");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewFacilitySports");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewFacilityOperatingHours");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewFacilityAmenities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewFacilityAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewFacility");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewCoreUtilities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "NewAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "MembershipRequestComments");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "MembershipRequestBackgroundJobs");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "MemberType");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FreezeConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FormUserResponse");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FieldTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FieldResponse");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FieldCategory");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FedrationEntityTypes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FedrationBranches");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityType");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityServices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityOperatingHours");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityLocation");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityImages");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "FacilityAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "EventDetails");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "EntityType");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "EntityMembers");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "EntityAdmins");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "DropdownValue");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "CountryCodes");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "CoreServices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "CoreFacilityServices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "ConfigurationFields");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "City");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "CancellationConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BranchServices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BranchOperatingHours");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BranchLocation");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BranchImages");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BookMarks");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Belts");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "AreaPrices");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "AreaConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "AdditionalConfigurations");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "AccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "AccessibilityFeatures");
        }
    }
}
