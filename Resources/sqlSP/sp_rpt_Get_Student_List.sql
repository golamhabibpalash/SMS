USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_rpt_Get_Students_List]    Script Date: 05-May-23 11:58:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Gh Palash>
-- Create date: <04 May 2023>
-- Description:	<Get all students for report>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_rpt_Get_Students_List] 
	@AcademicSessionId int,
	@AcademicClassId varchar(3),
	@AcademicSectionId varchar(3)
AS
BEGIN
	select 
	Convert(varchar(10),r.ClassRoll)[ClassRoll], 
	r.StudentName,
	Convert (varchar(1),r.AcademicClassId)[AcademicClassId],
	r.ClassName,
	Convert(varchar(2),r.AcademicSectionId)[AcademicSectionId],
	r.SectionName,
	r.SessionName, 
	r.FatherName, 
	r.MotherName,
	r.GuardianPhone,
	r.PhoneNo,
	r.BloodGroup,
	r.Gender,
	r.Religion,
	Case 
		when r.Status = 1 then 'Active' 
		else 'Inactive' end Status 
	from vw_rpt_student_info r
	where r.AcademicSessionId = @AcademicSessionId and
	@AcademicClassId is null or r.AcademicClassId = @AcademicClassId and
	@AcademicSectionId is null or r.AcademicSectionId = @AcademicSectionId
END
GO


