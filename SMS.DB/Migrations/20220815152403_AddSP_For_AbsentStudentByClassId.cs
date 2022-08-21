using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AddSP_For_AbsentStudentByClassId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp = @"USE [SMSDB]
                        GO
                        /****** Object:  StoredProcedure [dbo].[sp_attendance_get_absent_students]    Script Date: 8/15/2022 9:22:27 PM ******/
                        SET ANSI_NULLS ON
                        GO
                        SET QUOTED_IDENTIFIER ON
                        GO
                        CREATE Procedure [dbo].[sp_attendance_get_absent_students]
                        @academicSession int,
                        @studentClassId int,
                        @Date date
                        AS
                        BEGIN
	                        select s.* from Student s where s.AcademicClassId = @studentClassId and s.AcademicSessionId = @academicSession and s.ClassRoll not in (
	                        SELECT DISTINCT Convert(int,t.CardNo)
	                        FROM dbo.Tran_MachineRawPunch t
	                        left join Student s on s.ClassRoll = Convert(int,t.CardNo)
	                        where (Format(t.PunchDatetime,'yyyy-MM-dd')=@date) and (LEN(Convert(int,t.CardNo))=7)
	                        and s.AcademicClassId = @studentClassId)
                        END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp = @"DROP Procedure [dbo].[sp_attendance_get_absent_students]";
            migrationBuilder.Sql(sp);
        }
    }
}
