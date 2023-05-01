USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_attendance]    Script Date: 29-Apr-23 9:28:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER     view [dbo].[vw_rpt_student_attendance] 
as
select s.ClassRoll[CardNo],
s.Name[Name],
c.Name[Class_Designation],
s.PhoneNo[Phone],
(case
	when t.PunchDatetime is NOT NULL 
	Then t.PunchDatetime 
	END) as PunchTime,
s.GuardianPhone,
s.AcademicClassId,
s.AcademicSessionId,
s.AcademicSectionId,
t.Tran_MachineRawPunchId
from (select * from Student where Status=1) s
inner join AcademicClass as c on s.AcademicClassId=c.Id
left join Tran_MachineRawPunch t on s.ClassRoll = RIGHT(t.CardNo,7)
GO


