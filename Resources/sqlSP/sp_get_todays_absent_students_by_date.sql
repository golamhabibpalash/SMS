USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_get_todays_absent_students_by_date]    Script Date: 26-May-24 11:53:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_get_todays_absent_students_by_date]
@date varchar(10)
AS
BEGIN
	select * from student where convert(int,UniqueId) not in (
	(select convert(int,t.CardNo) from tran_MachineRawPunch t
	where FORMAT(t.PunchDatetime, 'dd-MM-yyyy') = @date 
	group by t.cardno)
	) and status = 1
END
