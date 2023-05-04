USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_payments_All]    Script Date: 04-May-23 6:32:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   view [dbo].[vw_rpt_payments_All]
as
SELECT 
Convert(varchar,s.ClassRoll)[ClassRoll],
s.Name[StudentName],
sec.Name[AcademicSection],
h.Name[PaymentType], 
p.ReceiptNo,
p.Remarks,
Convert(varchar(10),p.PaidDate)[PaidDate],
p.TotalPayment[TotalPayment],
Convert(varchar,s.AcademicClassId)[AcademicClassId],
Convert(varchar,s.AcademicSectionId)[AcademicSectionId],
c.Name[AcademicClassName]
from StudentPayment p
inner join StudentPaymentDetails as d on p.Id=d.StudentPaymentId
inner join StudentFeeHead as h on d.StudentFeeHeadId = h.Id
inner join Student as s on p.StudentId = s.Id
inner join AcademicClass as c on s.AcademicClassId = c.Id
left join AcademicSection as sec on s.AcademicSectionId = sec.Id;
GO


