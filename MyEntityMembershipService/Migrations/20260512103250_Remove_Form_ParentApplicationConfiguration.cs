using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Form_ParentApplicationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_ParentApplicationConfiguration_FormId",
                table: "ParentApplicationConfiguration");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "ParentApplicationConfiguration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormId",
                table: "ParentApplicationConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParentApplicationConfiguration_FormId",
                table: "ParentApplicationConfiguration",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentApplicationConfiguration_Forms_FormId",
                table: "ParentApplicationConfiguration",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");
        }
    }
}
