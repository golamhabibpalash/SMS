
USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_todays_absent_students_by_date]    Script Date: 18-Sep-23 3:41:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		GH Palash
-- Create date: 10 Dec 2022
-- Description:	SP for Get Students List by date
-- =============================================
CREATE PROCEDURE [dbo].[sp_get_todays_absent_employees_by_date]
@date varchar(10)
AS
BEGIN

	select * from Employee where Right(Phone,9) not in (
	(select t.CardNo from tran_MachineRawPunch t
	where len(t.cardno)=9 and
	FORMAT(t.PunchDatetime, 'dd-MM-yyyy') = @date
	group by t.cardno)
	) and status = 1;

END

