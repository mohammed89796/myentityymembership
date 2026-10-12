using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class Add_Self_Relation_Form : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentFormId",
                table: "Forms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_ParentFormId",
                table: "Forms",
                column: "ParentFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Forms_ParentFormId",
                table: "Forms",
                column: "ParentFormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Forms_ParentFormId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_ParentFormId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ParentFormId",
                table: "Forms");
        }
    }
}
