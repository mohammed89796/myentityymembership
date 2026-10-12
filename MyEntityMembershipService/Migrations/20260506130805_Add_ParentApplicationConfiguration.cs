using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class Add_ParentApplicationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParentApplicationConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentProceduresType = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ParentApplicationConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentApplicationConfiguration_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentApplicationConfiguration_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PackageRequestAuthourizations_PackageId",
            //    table: "PackageRequestAuthourizations",
            //    column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentApplicationConfiguration_FormId",
                table: "ParentApplicationConfiguration",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentApplicationConfiguration_PackageId",
                table: "ParentApplicationConfiguration",
                column: "PackageId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_PackageRequestAuthourizations_Packages_PackageId",
            //    table: "PackageRequestAuthourizations",
            //    column: "PackageId",
            //    principalTable: "Packages",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_PackageRequestAuthourizations_Packages_PackageId",
            //    table: "PackageRequestAuthourizations");

            migrationBuilder.DropTable(
                name: "ParentApplicationConfiguration");

            //migrationBuilder.DropIndex(
            //    name: "IX_PackageRequestAuthourizations_PackageId",
            //    table: "PackageRequestAuthourizations");
        }
    }
}
