using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantAdminRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantAdminBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantAdminId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GlobalTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAdminBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAdminBranches_TenantAdmins_TenantAdminId",
                        column: x => x.TenantAdminId,
                        principalTable: "TenantAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantAdminFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantAdminId = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GlobalTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAdminFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAdminFacilities_TenantAdmins_TenantAdminId",
                        column: x => x.TenantAdminId,
                        principalTable: "TenantAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantAdminSports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantAdminId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GlobalTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAdminSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAdminSports_TenantAdmins_TenantAdminId",
                        column: x => x.TenantAdminId,
                        principalTable: "TenantAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TenantAdminVenues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantAdminId = table.Column<int>(type: "int", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModificationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GlobalTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAdminVenues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAdminVenues_TenantAdmins_TenantAdminId",
                        column: x => x.TenantAdminId,
                        principalTable: "TenantAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequestAccess_BranchId",
                table: "MembershipRequestAccess",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequestAccess_FacilityId",
                table: "MembershipRequestAccess",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipRequestAccess_VenueId",
                table: "MembershipRequestAccess",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAdminBranches_TenantAdminId",
                table: "TenantAdminBranches",
                column: "TenantAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAdminFacilities_TenantAdminId",
                table: "TenantAdminFacilities",
                column: "TenantAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAdminSports_TenantAdminId",
                table: "TenantAdminSports",
                column: "TenantAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAdminVenues_TenantAdminId",
                table: "TenantAdminVenues",
                column: "TenantAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipRequestAccess_Branches_BranchId",
                table: "MembershipRequestAccess",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_Branches_BranchId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewFacility_FacilityId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_MembershipRequestAccess_NewVenue_VenueId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropTable(
                name: "TenantAdminBranches");

            migrationBuilder.DropTable(
                name: "TenantAdminFacilities");

            migrationBuilder.DropTable(
                name: "TenantAdminSports");

            migrationBuilder.DropTable(
                name: "TenantAdminVenues");

            migrationBuilder.DropIndex(
                name: "IX_MembershipRequestAccess_BranchId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropIndex(
                name: "IX_MembershipRequestAccess_FacilityId",
                table: "MembershipRequestAccess");

            migrationBuilder.DropIndex(
                name: "IX_MembershipRequestAccess_VenueId",
                table: "MembershipRequestAccess");
        }
    }
}
