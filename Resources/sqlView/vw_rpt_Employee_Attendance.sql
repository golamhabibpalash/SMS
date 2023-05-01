USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_Employee_Attendance]    Script Date: 01-May-23 9:58:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER       View [dbo].[vw_rpt_Employee_Attendance]
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
inner join Employee as e on t.CardNo=Right(e.Phone,9)
inner join Designation as d on e.DesignationId=d.Id
GO


