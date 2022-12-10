using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class SetupMobileSMSModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbsentNotification",
                table: "SetupMobileSMSs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CheckInSMSSummary",
                table: "SetupMobileSMSs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "AbsentNotification",
                table: "SetupMobileSMSs");

            migrationBuilder.DropColumn(
                name: "CheckInSMSSummary",
                table: "SetupMobileSMSs");
        }
    }
}
