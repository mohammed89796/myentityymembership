using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Name_ParentApplicationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentApplicationConfiguration_Packages_PackageId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_ParentApplicationConfiguration_PackageId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ParentApplicationConfiguration");

            migrationBuilder.CreateIndex(
                name: "IX_ParentApplicationConfiguration_PackageId",
                table: "ParentApplicationConfiguration",
                column: "PackageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentApplicationConfiguration_Packages_PackageId",
                table: "ParentApplicationConfiguration",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentApplicationConfiguration_Packages_PackageId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_ParentApplicationConfiguration_PackageId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ParentApplicationConfiguration",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ParentApplicationConfiguration_PackageId",
                table: "ParentApplicationConfiguration",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentApplicationConfiguration_Packages_PackageId",
                table: "ParentApplicationConfiguration",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
