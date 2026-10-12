using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Venues",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueSlots",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueReviews",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueOperatingHours",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueLocation",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueImages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueBookingTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueBookingSlot",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "VenueBooking",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "UsersChoices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Units",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "UnitTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TurfTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TournamentResults",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TermsOfServices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Tenants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantReviews",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAreaAdmins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantArea",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdmins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdminVenues",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdminSports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdminRole",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdminFacilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TenantAdminBranches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Teams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "TeamMembers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "SubMemberRemovalConfiguration",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "SubMemberCategories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "SubMemberAdditionalConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "State",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "StadFacilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "SportsAccommodated",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Sports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Shares",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Series",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RequestTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RequestTypeFields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RequestTypeCategory",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RequestStatusHistory",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RenwalOption",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RenewalConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "RemovalConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PreRegisteredUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Posts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PostTags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Polls",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PaymentRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PaymentInvoiceTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PaymentBatches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "ParentApplicationConfiguration",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Packages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PackageScopes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PackageScopeAssignments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PackageRequestAuthourizations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PackageProducts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PackageConfigurationSections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "PaakageMemberForms",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Organizations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Operations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueUtilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueTurfTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenuePricingRules",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueDiscountTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueBookingConfiguration",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueAmenities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenueAccessibilityOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewVenue",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewPaymentRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewMembershipConfiguration",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewFacilitySports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewFacilityOperatingHours",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewFacilityAmenities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewFacilityAccessibilityOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewFacility",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewCoreUtilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "NewAccessibilityOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Memberships",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipRequestProduct",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipRequestComments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipRequestBackgroundJobs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipRequestAccess",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipProduct",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MembershipAccesses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MemberType",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MemberSportLevels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "MemberRequestTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Likes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "InjuryReports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "HistoricalTournament",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Galleries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FreezeConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Forms",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FormUserResponse",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FormMemberType",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FormFields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Fields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FieldTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FieldResponse",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FieldCategory",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FedrationEntityTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FedrationBranches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityType",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityServices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityOperatingHours",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityLocation",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityImages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "FacilityAccessibilityOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Facilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "EventDetails",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "EntityType",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "EntityMembers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "EntityAdmins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "DropdownValue",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "CountryCodes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Country",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "CoreServices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "CoreFacilityServices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Configurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "ConfigurationFields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "ConfigrationFieldResponses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Comments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "City",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Choices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "CancellationConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Branches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BranchServices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BranchOperatingHours",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BranchLocation",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BranchImages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BookMarks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Belts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Batches",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "BankAccounts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Attainments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AttainmentCategories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Areas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AreaPrices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AreaConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Amenities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Albums",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AdditionalConfigurations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AccessibilityOptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "AccessibilityFeatures",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueSlots");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueReviews");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueOperatingHours");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueLocation");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueBookingTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueBookingSlot");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "VenueBooking");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "UsersChoices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "UnitTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TurfTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TournamentResults");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TermsOfServices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantReviews");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAreaAdmins");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantArea");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdmins");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdminVenues");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdminSports");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdminRole");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdminFacilities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TenantAdminBranches");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "SubMemberRemovalConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "SubMemberCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "SubMemberAdditionalConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "State");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "StadFacilities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "SportsAccommodated");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Sports");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RequestTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RequestTypeFields");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RequestTypeCategory");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RequestStatusHistory");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RenwalOption");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RenewalConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "RemovalConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PreRegisteredUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PostTags");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PaymentRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PaymentInvoiceTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PaymentBatches");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PackageScopes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PackageScopeAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PackageRequestAuthourizations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PackageProducts");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PackageConfigurationSections");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "PaakageMemberForms");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueUtilities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueTurfTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenuePricingRules");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueDiscountTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueBookingConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueAmenities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenueAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewVenue");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewPaymentRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewMembershipConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewFacilitySports");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewFacilityOperatingHours");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewFacilityAmenities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewFacilityAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewFacility");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewCoreUtilities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "NewAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipRequestProduct");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipRequestComments");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipRequestBackgroundJobs");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipRequestAccess");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipProduct");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MembershipAccesses");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MemberType");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MemberSportLevels");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "MemberRequestTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FreezeConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FormUserResponse");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FormMemberType");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FormFields");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FieldTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FieldResponse");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FieldCategory");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FedrationEntityTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FedrationBranches");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityType");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityServices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityOperatingHours");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityLocation");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "FacilityAccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "EventDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "EntityType");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "EntityMembers");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "EntityAdmins");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "DropdownValue");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "CountryCodes");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "CoreServices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "CoreFacilityServices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "ConfigurationFields");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "ConfigrationFieldResponses");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "City");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "CancellationConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BranchServices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BranchOperatingHours");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BranchLocation");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BranchImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BookMarks");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Belts");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Attainments");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AttainmentCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AreaPrices");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AreaConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Amenities");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AdditionalConfigurations");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AccessibilityOptions");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "AccessibilityFeatures");
        }
    }
}
