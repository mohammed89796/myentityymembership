using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLinks",
                table: "Tenants",
                type: "json",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialMediaLinks",
                table: "Tenants");
        }
    }
}
