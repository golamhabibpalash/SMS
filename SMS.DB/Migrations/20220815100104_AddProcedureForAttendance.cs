using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AddProcedureForAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedureForAttended = @"USE [SMSDB]
                                            GO
                                            /****** Object:  StoredProcedure [dbo].[sp_attendance_get_attended_Students]    Script Date: 8/15/2022 4:02:48 PM ******/
                                            SET ANSI_NULLS ON
                                            GO
                                            SET QUOTED_IDENTIFIER ON
                                            GO
                                            CREATE PROCEDURE [dbo].[sp_attendance_get_attended_Students]
                                            @academicSessionId int,
                                            @classId int,
                                            @date date
                                            AS
                                            BEGIN
	                                            select s.* from Student s where s.AcademicClassId = @classId and s.AcademicSessionId = @academicSessionId and s.ClassRoll in (
	                                            SELECT DISTINCT Convert(int,t.CardNo)
	                                            FROM dbo.Tran_MachineRawPunch t
	                                            left join Student s on s.ClassRoll = Convert(int,t.CardNo)
	                                            where (Format(t.PunchDatetime,'yyyy-MM-dd')=@date) and (LEN(Convert(int,t.CardNo))=7)
	                                            and s.AcademicClassId = @classId)
                                            END";
            migrationBuilder.Sql(procedureForAttended);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedureForAttended = @"DROP PROCEDURE [dbo].[sp_attendance_get_attended_Students]";
            migrationBuilder.Sql(procedureForAttended);
        }
    }
}
