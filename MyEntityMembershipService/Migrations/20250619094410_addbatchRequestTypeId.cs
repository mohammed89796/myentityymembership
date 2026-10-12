using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class addbatchRequestTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "RoleId",
            //    table: "TenantAdmins");

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_RequestTypeId",
                table: "Batches",
                column: "RequestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_RequestTypes_RequestTypeId",
                table: "Batches",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_RequestTypes_RequestTypeId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_RequestTypeId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "RequestTypeId",
                table: "Batches");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "TenantAdmins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
