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
CREATE PROCEDURE sp_get_scheduled_paid_by_id 
	@studId int
AS
BEGIN
	SELECT PaymentType, COUNT(*) AS PaymentCount, SUM(PaidAmount) AS PaidAmount
	FROM vw_schedule_wise_stu_paid
	WHERE StudentId = @studId
	GROUP BY PaymentType;
END
GO
