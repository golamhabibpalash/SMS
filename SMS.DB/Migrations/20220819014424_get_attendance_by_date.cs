using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class get_attendance_by_date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			string sql = @"USE SMSDB
							SET ANSI_NULLS ON
							GO
							SET QUOTED_IDENTIFIER ON
							GO
							-- =============================================
							-- Author: GHPalash
							-- Create date: 18 Aug 2022
							-- Description:	Getting data for attendance report
							-- =============================================
							CREATE PROCEDURE sp_get_attendance_by_date
								@attendanceFor VARCHAR(10), --Employee/Student
								@date VARCHAR(10),
								@attendanceType VARCHAR(10), --attended/absent/all
								@aSessionId INT = null,
								@aClassId INT = null
							AS
							BEGIN
								IF @attendanceFor = 'employees'
									BEGIN
										IF @attendanceType = 'attended'
											BEGIN
												SELECT e.EmployeeName,d.DesignationName, e.Phone,FORMAT(t.PunchDatetime,'hh:mm tt') [Punch Time] 
												FROM Employee e
												LEFT JOIN Designation d ON e.DesignationId = d.Id
												LEFT JOIN (
												SELECT tm.CardNo,MIN(tm.PunchDateTime)PunchDatetime FROM Tran_MachineRawPunch tm
												WHERE FORMAT(tm.PunchDatetime,'yyyy-MM-dd')=@date GROUP BY tm.CardNo
												) t 
												ON RIGHT(e.Phone,9)=CONVERT(int,t.CardNo)
												WHERE RIGHT(e.Phone,9) in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=9)
												ORDER BY t.PunchDatetime
											END
										ELSE IF @attendanceType = 'absent'
											BEGIN
												SELECT e.EmployeeName,d.DesignationName, e.Phone,'Absent' [Punch Time] FROM Employee e
												LEFT JOIN Designation d ON e.DesignationId = d.Id
												LEFT JOIN (
												SELECT tm.CardNo,MIN(tm.PunchDateTime)PunchDatetime FROM Tran_MachineRawPunch tm
												WHERE FORMAT(tm.PunchDatetime,'yyyy-MM-dd')=@date GROUP BY tm.CardNo
												) t 
												ON RIGHT(e.Phone,9)=CONVERT(int,t.CardNo)
												WHERE RIGHT(e.Phone,9) not in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=9)
												ORDER BY e.JoiningDate
											END	
										ELSE
											BEGIN
												SELECT emp.* from
												(
												SELECT e.EmployeeName,d.DesignationName, e.Phone,FORMAT(t.PunchDatetime,'hh:mm tt') [Punch Time] 
												FROM Employee e
												LEFT JOIN Designation d ON e.DesignationId = d.Id
												LEFT JOIN (
												SELECT tm.CardNo,MIN(tm.PunchDateTime)PunchDatetime FROM Tran_MachineRawPunch tm
												WHERE FORMAT(tm.PunchDatetime,'yyyy-MM-dd')=@date GROUP BY tm.CardNo
												) t 
												ON RIGHT(e.Phone,9)=CONVERT(int,t.CardNo)
												WHERE RIGHT(e.Phone,9) in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=9)
												UNION
												SELECT e.EmployeeName,d.DesignationName, e.Phone,'Absent' [Punch Time] FROM Employee e
												LEFT JOIN Designation d ON e.DesignationId = d.Id
												LEFT JOIN (
												SELECT tm.CardNo,MIN(tm.PunchDateTime)PunchDatetime FROM Tran_MachineRawPunch tm
												WHERE FORMAT(tm.PunchDatetime,'yyyy-MM-dd')=@date GROUP BY tm.CardNo
												) t 
												ON RIGHT(e.Phone,9)=CONVERT(int,t.CardNo)
												WHERE RIGHT(e.Phone,9) not in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=9)
												) emp ORDER BY emp.[Punch Time]		
											END
									END
								ELSE
									BEGIN
										IF @attendanceType = 'attended'
											BEGIN
												SELECT s.ClassRoll, 
												s.Name[Student Name], 
												aClass.Name[Class],
												s.GuardianPhone,
												FORMAT(t.Punchtime,'hh:mm tt')[Punch Time] FROM Student s
												LEFT JOIN AcademicClass aClass ON s.AcademicClassId = aClass.Id
												LEFT JOIN (
													SELECT 
													CONVERT(int,tm.CardNo)[CardNo],
													MIN(tm.PunchDatetime)[Punchtime] 
													FROM Tran_MachineRawPunch tm 
													WHERE FORMAT(tm.PunchDatetime, 'yyyy-MM-dd')=@date 
													GROUP BY CONVERT(int,tm.CardNo)
													) t ON s.ClassRoll = CONVERT(int,t.CardNo)
												WHERE s.ClassRoll in (
												SELECT DISTINCT (CONVERT(INT,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=7) 
												and(@aSessionId IS NULL OR s.AcademicSessionId=@aSessionId)
												and(@aClassId IS NULL OR s.AcademicClassId=@aClassId)
												ORDER BY aClass.ClassSerial
											END
										ELSE IF @attendanceType = 'absent'
											BEGIN
												SELECT s.ClassRoll, 
												s.Name, 
												aClass.Name,
												s.GuardianPhone,
												'Absent'[Punch Time] FROM Student s
												LEFT JOIN AcademicClass aClass ON s.AcademicClassId = aClass.Id	
												WHERE s.ClassRoll not in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=7) 
												and(@aSessionId IS NULL OR s.AcademicSessionId=@aSessionId)
												and(@aClassId IS NULL OR s.AcademicClassId=@aClassId)
												ORDER BY aClass.ClassSerial
											END
										ELSE
											BEGIN
												SELECT stu.* FROM (
												SELECT s.ClassRoll, 
												s.Name[Student Name], 
												aClass.Name[Academic Class],
												s.GuardianPhone,
												FORMAT(t.Punchtime,'hh:mm tt')[Punch Time] FROM Student s
												LEFT JOIN AcademicClass aClass ON s.AcademicClassId = aClass.Id
												LEFT JOIN (
													SELECT 
													CONVERT(int,tm.CardNo)[CardNo],
													MIN(tm.PunchDatetime)[Punchtime] 
													FROM Tran_MachineRawPunch tm 
													WHERE FORMAT(tm.PunchDatetime, 'yyyy-MM-dd')=@date 
													GROUP BY CONVERT(int,tm.CardNo)
													) t ON s.ClassRoll = CONVERT(int,t.CardNo)
												WHERE s.ClassRoll in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=7) 
												and(@aSessionId IS NULL OR s.AcademicSessionId=@aSessionId)
												and(@aClassId IS NULL OR s.AcademicClassId=@aClassId)					
												UNION
												SELECT s.ClassRoll, 
												s.Name, 
												aClass.Name,
												s.GuardianPhone,
												'Absent'[Punch Time] FROM Student s
												LEFT JOIN AcademicClass aClass ON s.AcademicClassId = aClass.Id	
												WHERE s.ClassRoll not in (
												SELECT DISTINCT (CONVERT(int,t.CardNo)) FROM Tran_MachineRawPunch t
												WHERE FORMAT(t.PunchDatetime, 'yyyy-MM-dd') = @date and
												LEN(CONVERT(int,t.CardNo))=7) 
												and(@aSessionId IS NULL OR s.AcademicSessionId=@aSessionId)
												and(@aClassId IS NULL OR s.AcademicClassId=@aClassId)
												) stu ORDER BY stu.ClassRoll
											END			
									END
							END
							GO";
			migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string sql = @"drop procedure sp_get_attendance_by_date";
			migrationBuilder.Sql(sql);
        }
    }
}
