using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class TeamCoachNotRequried : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CoachID",
                table: "Teams",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SportID",
                table: "Teams",
                column: "SportID");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Sports_SportID",
                table: "Teams",
                column: "SportID",
                principalTable: "Sports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Sports_SportID",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SportID",
                table: "Teams");

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "CoachID",
                keyValue: null,
                column: "CoachID",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CoachID",
                table: "Teams",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
