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
-- Author:		GH Palash
-- Create date: 13 Sept 2023
-- Description:	Attendance list of student by Month and StudentId
-- =============================================
ALTER PROCEDURE sp_get_Attendance_by_Month_SingleStudent 
	@studentId int,
	@monthId int
AS
BEGIN
	SELECT MAX(Tran_MachineRawPunchId) AS Tran_MachineRawPunchId,
       CardNo,
       CAST(PunchDatetime AS DATE) AS PunchDatetime,
       MAX(P_Day) AS P_Day,
       MAX(ISManual) AS ISManual,
       MAX(PayCode) AS PayCode,
       MAX(MachineNo) AS MachineNo
FROM [dbo].[Tran_MachineRawPunch] t
inner join Student s on t.CardNo = s.ClassRoll
where s.Id=@studentId and MONTH(t.PunchDatetime)=@monthId
GROUP BY CAST(PunchDatetime AS DATE), CardNo;

	
END
GO
