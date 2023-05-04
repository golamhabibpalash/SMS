USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_Get_Student_Payments]    Script Date: 04-May-23 4:21:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER         PROCEDURE [dbo].[sp_Get_Student_Payments] 
	@fromDate varchar(10),
	@toDate  varchar(10),
	@academicClassId varchar(1),
	@academicSectionId varchar(2)
AS
BEGIN
	
 SELECT v.ClassRoll,v.StudentName,
 v.AcademicSection,
 v.PaymentType,
 v.ReceiptNo,
 v.Remarks,
 v.PaidDate,
 v.TotalPayment,
 v.AcademicClassId,
 v.AcademicSectionId,
 v.AcademicClassName 
 from vw_rpt_student_payments v
 Where (@academicClassId is null OR v.AcademicClassId = @academicClassId)
 and ( @academicSectionId is null OR v.AcademicSectionId = @academicSectionId)
 and v.PaidDate between Convert(date,@fromDate) and Convert(date,@toDate)
END
GO


