USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_schedule_wise_stu_paid]    Script Date: 18-Sep-23 12:24:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER view [dbo].[vw_schedule_wise_stu_paid]
as
select s.Id[StudentId],s.ClassRoll,s.name[StudentName], c.Id[ClassId], c.Name[ClassName],f.Name[PaymentType], d.PaidAmount,p.TotalPayment,f.IsResidential
from StudentPaymentDetails d
inner join StudentPayment p on d.StudentPaymentId = p.Id 
left join student s on p.StudentId = s.Id
left join AcademicClass c on s.AcademicClassId = c.Id
left join StudentFeeHead f on d.StudentFeeHeadId=f.Id;
GO


