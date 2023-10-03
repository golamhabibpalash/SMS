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
-- Create date: 23 Sept 23
-- Description:	This SP is created to get Result for Marksheet report
-- =============================================
CREATE PROCEDURE sp_rpt_get_result_by_examGroup_n_Class
@examGroupId int,
@classId int
AS
BEGIN
	select t.* from vw_rpt_student_wise_marksheet t where t.ExamGroupId=@examGroupId and t.AcademicClassId=@classId
END
GO
