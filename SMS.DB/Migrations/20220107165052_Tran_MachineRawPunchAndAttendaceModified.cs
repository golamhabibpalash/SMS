using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class Tran_MachineRawPunchAndAttendaceModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceDate",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "InTime",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "IsPresent",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "OutTime",
                table: "Attendances",
                newName: "PunchDatetime");

            migrationBuilder.AddColumn<string>(
                name: "CardNo",
                table: "Attendances",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosingTime",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LateStartAfter",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MachineNo",
                table: "Attendances",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartingTime",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNo",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "LateStartAfter",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "MachineNo",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "StartingTime",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "PunchDatetime",
                table: "Attendances",
                newName: "OutTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "Attendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "InTime",
                table: "Attendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPresent",
                table: "Attendances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
