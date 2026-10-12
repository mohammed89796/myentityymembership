using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEntityMembershipService.Migrations
{
    /// <inheritdoc />
    public partial class TournamentResultUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "TournamentResults",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CairoRank",
                table: "TournamentResults",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "HistoricalTournament",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "HistoricalTournament",
                newName: "CreatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "SwimmerPhoneNumber",
                table: "TournamentResults",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SwimmerNationalId",
                table: "TournamentResults",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "TournamentResults",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "TournamentResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TournamentResults",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "TournamentResults",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InjuryReports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "InjuryReports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "InjuryReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InjuryReports",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "InjuryReports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "InjuryReports",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<int>(
                name: "ClubRank",
                table: "HistoricalTournament",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "HistoricalTournament",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GlobalTenantId",
                table: "HistoricalTournament",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HistoricalTournament",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "HistoricalTournament",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "TournamentResults");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "TournamentResults");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TournamentResults");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "TournamentResults");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "InjuryReports");

            migrationBuilder.DropColumn(
                name: "ClubRank",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "GlobalTenantId",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "HistoricalTournament");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "HistoricalTournament");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "TournamentResults",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "TournamentResults",
                newName: "CairoRank");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "HistoricalTournament",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "HistoricalTournament",
                newName: "City");

            migrationBuilder.UpdateData(
                table: "TournamentResults",
                keyColumn: "SwimmerPhoneNumber",
                keyValue: null,
                column: "SwimmerPhoneNumber",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "SwimmerPhoneNumber",
                table: "TournamentResults",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "TournamentResults",
                keyColumn: "SwimmerNationalId",
                keyValue: null,
                column: "SwimmerNationalId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "SwimmerNationalId",
                table: "TournamentResults",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "HistoricalTournament",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
