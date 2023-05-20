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
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_Get_Current_Student_List 
	@academicClassId int,
	@academicSectionId int
AS
BEGIN
	select s.Id,
	s.ClassRoll,
	s.Photo,
	s.Name[StudentName],
	s.NameBangla,
	c.Name[ClassName],
	sec.Name[SectionName],
	s.PhoneNo,
	ses.Name[SessionName], 
	g.Name[Gender],
	s.Status
	from student s
	inner join AcademicClass c on s.AcademicClassId= c.Id
	left join AcademicSection sec on s.AcademicSectionId = sec.Id
	inner join AcademicSession ses on s.AcademicSessionId = ses.Id
	inner join Gender g on s.GenderId= g.Id
	where (@academicClassId is null or s.AcademicClassId = @academicClassId) and
	(@academicSectionId is null or s.AcademicSectionId = @academicSectionId)
	order by s.ClassRoll
END
GO
