-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_rpt_payment_receipt
@paymentId int
AS
BEGIN
	select p.ReceiptNo,s.Name[Student_Name],c.Name[Class_Name],p.PaidDate,s.ClassRoll,sec.Name[Section_Name],sfh.Name[Fee_Head], d.PaidAmount,p.TotalPayment from StudentPayment p
	inner join StudentPaymentDetails d on p.Id = d.StudentPaymentId
	inner join Student s on p.StudentId = s.Id
	inner join AcademicClass c on s.AcademicClassId = c.Id
	left join AcademicSection sec on s.AcademicSectionId = sec.Id
	left join StudentFeeHead sfh on d.StudentFeeHeadId = sfh.Id
	where p.Id = @paymentId;
END
GO
