using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AcademicExamTypeModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountInAYear",
                table: "AcademicExamTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AcademicExamTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountInAYear",
                table: "AcademicExamTypes");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AcademicExamTypes");
        }
    }
}
