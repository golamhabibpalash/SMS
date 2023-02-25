using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class addNewView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create view vw_rpt_student_info as
SELECT s.ClassRoll, s.Name 'StudentName', c.Name 'ClassName', acSession.Name 'SessionName', s.FatherName, s.MotherName, s.GuardianPhone, s.PhoneNo, g.Name 'Gender', r.Name 'Religion', s.Status
FROM Student s 
INNER JOIN academicClass c ON s.AcademicClassId = c.Id 
INNER JOIN academicSession acSession ON s.AcademicSessionId = acSession.Id
INNER JOIN Gender g ON s.GenderId = g.Id 
INNER JOIN Religion r ON s.religionId = r.Id
ORDER BY c.ClassSerial, s.ClassRoll offset 0 rows");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .Sql(@"drop view vw_rpt_student_info");
        }
    }
}
