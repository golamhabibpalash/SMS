USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Payments_Get_All_by_Roll]    Script Date: 04-May-23 6:35:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[sp_Payments_Get_All_by_Roll] 
	@classRoll int
AS
BEGIN
	select p.ReceiptNo,
	FORMAT(p.PaidDate,'dd MMM yyyy')[PaidDate],
	p.PaymentTypeName,
	p.Remarks,
	p.TotalPayment 
	from vw_rpt_paymentInfo p 
	where p.ClassRoll = @classRoll order by p.PaidDate
END
