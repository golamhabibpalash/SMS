USE [SMSDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Get_Current_Student_List]    Script Date: 08-Jan-24 11:23:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_Get_Current_Student_List] 
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
	s.Status,
	c.ClassSerial,
	s.IsResidential,
	s.UniqueId
	from student s
	inner join AcademicClass c on s.AcademicClassId= c.Id
	left join AcademicSection sec on s.AcademicSectionId = sec.Id
	inner join AcademicSession ses on s.AcademicSessionId = ses.Id
	inner join Gender g on s.GenderId= g.Id
	where (@academicClassId is null or s.AcademicClassId = @academicClassId) and
	(@academicSectionId is null or s.AcademicSectionId = @academicSectionId)
	order by s.ClassRoll
END
