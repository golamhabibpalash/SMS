USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_attendance]    Script Date: 1/16/2024 5:10:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER     view [dbo].[vw_rpt_student_attendance]
as
select s.classroll[CardNo],
s.Name[Name],
c.Name[Class_Designation],
s.PhoneNo[Phone],
t.PunchDatetime[PunchTime],
s.GuardianPhone,
s.AcademicClassId,
s.AcademicSectionId,
s.AcademicSessionId,
t.Tran_MachineRawPunchId
from Student s
join Tran_MachineRawPunch t on s.UniqueId=t.CardNo
inner join AcademicClass c on s.AcademicClassId = c.Id
GO


