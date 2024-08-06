USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_attendance]    Script Date: 23-Mar-24 1:22:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER     view [dbo].[vw_rpt_student_attendance]
as
select s.UniqueId[CardNo],
s.classroll[ClassRoll],
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
join Tran_MachineRawPunch t on cast(s.UniqueId as int)=cast(t.CardNo as int)
inner join AcademicClass c on s.AcademicClassId = c.Id
GO


