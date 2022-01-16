using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AttendaceTableModified1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateStartAfter",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "LateTime",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateTime",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "LateStartAfter",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
