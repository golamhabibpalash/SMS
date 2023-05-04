USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_Get_Payments_by_Roll]    Script Date: 04-May-23 5:42:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Get_Payments_Single_Student] 
	@classRoll int,
	@fromDate varchar(10),
	@toDate varchar(10)
AS
BEGIN
	select v.ReceiptNo,
	v.PaidDate,
	v.PaymentType[PaymentTypeName],
	v.TotalPayment,
	v.Remarks
	from vw_rpt_payments_all v
	where v.ClassRoll = @classRoll and
	CONVERT(date,v.PaidDate) between @fromDate and @toDate
END
GO


