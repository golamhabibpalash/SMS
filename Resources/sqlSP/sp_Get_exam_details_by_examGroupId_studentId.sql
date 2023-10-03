use SMSDB;
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
-- Author:		<GH Palash>
-- Create date: <19 Sept 2023>
-- Description:	<TO Get exam details list by group and studentId >
-- =============================================
CREATE PROCEDURE sp_Get_exam_details_by_examGroupId_studentId 
	@examGroupId int,
	@studentId int
AS
BEGIN
	select d.* from AcademicExamDetails d
	left join AcademicExams e on d.AcademicExamId = e.Id
	where d.StudentId =@studentId and e.AcademicExamGroupId =@examGroupId;
END
GO
