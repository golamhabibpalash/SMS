using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AcademicSubjectModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "AcademicSubject");

            migrationBuilder.AddColumn<string>(
                name: "SubjectFor",
                table: "AcademicSubject",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectFor",
                table: "AcademicSubject");

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "AcademicSubject",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
