USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Get_CheckOut_Data]    Script Date: 26-Jan-24 9:08:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		GH Palash
-- Create date: 03 Dec 2022
-- Description:	This procedure for Employees Checkin attendance
-- =============================================
ALTER PROCEDURE [dbo].[sp_Get_CheckOut_Data] 
	@date varchar(10)
AS
BEGIN
	select p.* from Tran_MachineRawPunch p,
	(select t.CardNo,MAX(t.PunchDatetime)'time' from Tran_MachineRawPunch t
	where FORMAT(t.PunchDatetime,'dd-MM-yyyy') = @date and FORMAT(t.PunchDatetime,'HH:mm:ss') > (select CONVERT(TIME, p.ParamValue, 108) from ParamBusConfigs p where p.ParamSL=3)
	group by t.CardNo) cn
	where p.CardNo = cn.CardNo and p.PunchDatetime = cn.time
	order by p.CardNo;
END