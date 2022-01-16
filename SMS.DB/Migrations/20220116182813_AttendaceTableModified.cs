using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AttendaceTableModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateStartAfter",
                table: "Institute");

            migrationBuilder.AddColumn<string>(
                name: "LateTime",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateTime",
                table: "Institute");

            migrationBuilder.AddColumn<int>(
                name: "LateStartAfter",
                table: "Institute",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
