USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_Employee_Attendance]    Script Date: 1/28/2024 12:11:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER       View [dbo].[vw_rpt_Employee_Attendance]
as
select t.CardNo,
e.EmployeeName[Name],
d.DesignationName[Class_Designation],
e.Phone,
t.PunchDatetime[PunchTime],
e.NomineePhone[GuardianPhone],
d.Id[DesignationId],
t.Tran_MachineRawPunchId
from Tran_MachineRawPunch t
inner join Employee as e on CAST(t.CardNo as int)=e.Id
inner join Designation as d on e.DesignationId=d.Id
GO


