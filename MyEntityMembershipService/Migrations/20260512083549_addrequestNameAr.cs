using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class addrequestNameAr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "RequestTypes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SubMemberCategories_MemberTypeId",
                table: "SubMemberCategories",
                column: "MemberTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubMemberCategories_MemberType_MemberTypeId",
                table: "SubMemberCategories",
                column: "MemberTypeId",
                principalTable: "MemberType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubMemberCategories_MemberType_MemberTypeId",
                table: "SubMemberCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubMemberCategories_MemberTypeId",
                table: "SubMemberCategories");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "RequestTypes");
        }
    }
}
