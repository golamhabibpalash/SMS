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
-- Create date: 10 Feb 2024
-- Description:	SP for get amount by SL
-- =============================================
CREATE PROCEDURE sp_get_amount_by_classFee_sl 
	-- Add the parameters for the stored procedure here
	@uniqueId varchar(10),
	@sl int
AS
BEGIN	
	select sum(t.Amount) 
	from ClassFeeList t
	left join StudentFeeHead f on t.StudentFeeHeadId = f.Id
	left join (select s.AcademicClassId, s.IsResidential from student s where s.UniqueId=@uniqueId) s on t.AcademicClassId = s.AcademicClassId 
	where t.AcademicSessionId = 4 and 
	t.AcademicClassId= s.AcademicClassId
	and f.IsResidential = s.IsResidential
	and  t.SL = @sl;
END
GO
