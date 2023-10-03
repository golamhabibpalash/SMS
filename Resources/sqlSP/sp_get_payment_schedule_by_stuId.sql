USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_payment_schedule_by_stuId]    Script Date: 18-Sep-23 12:27:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_payment_schedule_by_stuId] 
	@studId int
AS
BEGIN
	select v.ClassId,v.PaymentType,v.Amount,v.YearlyFrequency,v.IsResidential
	from vw_get_student_payment_schedule_list v 
	where v.classId =(select s.AcademicClassId from Student s where s.Id=@studId)
	and v.sessionId = (select s.academicSessionId from student s where s.Id=@studId)
END
