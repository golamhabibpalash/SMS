using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AcademicExamModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthId",
                table: "AcademicExams",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthId",
                table: "AcademicExams");
        }
    }
}
