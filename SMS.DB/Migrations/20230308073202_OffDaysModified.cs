using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class OffDaysModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OffDayDate",
                table: "OffDays",
                newName: "OffDayStartingDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "OffDayEndDate",
                table: "OffDays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OffDayName",
                table: "OffDays",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalDays",
                table: "OffDays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffDayEndDate",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "OffDayName",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "TotalDays",
                table: "OffDays");

            migrationBuilder.RenameColumn(
                name: "OffDayStartingDate",
                table: "OffDays",
                newName: "OffDayDate");
        }
    }
}
