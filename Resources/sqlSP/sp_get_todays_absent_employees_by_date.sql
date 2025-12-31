USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_todays_absent_employees_by_date]    Script Date: 26-Jan-24 10:53:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		GH Palash
-- Create date: 18 Sept 2023
-- Description:	SP for Get Students List by date
-- =============================================
ALTER PROCEDURE [dbo].[sp_get_todays_absent_employees_by_date]
@date varchar(10)
AS
BEGIN

	select * from Employee where Id not in (
	(select CONVERT(int, t.CardNo)as 'Cardno' from tran_MachineRawPunch t 
	where FORMAT(t.PunchDatetime, 'dd-MM-yyyy') = @date
	group by t.cardno)
	) and status = 1;

END

