using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class StartingTImeEndTimeLateTimeAddedInInstitute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISO",
                table: "Institute",
                newName: "StartingTime");

            migrationBuilder.AddColumn<string>(
                name: "ClosingTime",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LateStartAfter",
                table: "Institute",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "LateStartAfter",
                table: "Institute");

            migrationBuilder.RenameColumn(
                name: "StartingTime",
                table: "Institute",
                newName: "ISO");
        }
    }
}
