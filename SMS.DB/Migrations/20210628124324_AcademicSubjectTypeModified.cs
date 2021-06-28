using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AcademicSubjectTypeModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectTypeId",
                table: "AcademicSubjectType",
                newName: "SubjectTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectTypeName",
                table: "AcademicSubjectType",
                newName: "SubjectTypeId");
        }
    }
}
