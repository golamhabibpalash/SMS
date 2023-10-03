USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_get_student_payment_schedule_list]    Script Date: 18-Sep-23 12:28:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER VIEW [dbo].[vw_get_student_payment_schedule_list] 
AS
select 
c.Id[ClassId],
c.Name[ClassName],
sFee.Name[PaymentType],
cFee.Amount,
sFee.Repeatedly,
sFee.YearlyFrequency,
aSession.Id[SessionId],
sFee.IsResidential
from ClassFeeList cFee
left join StudentFeeHead sFee on cFee.StudentFeeHeadId = sFee.Id
left join AcademicClass c on cFee.AcademicClassId=c.Id
left join AcademicSession aSession on cFee.AcademicSessionId = aSession.Id
--order by aSession.Id desc,c.Id,sFee.Id;
GO


