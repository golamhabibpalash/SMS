USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_Admit_Card_Info]    Script Date: 21-May-23 11:10:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER   view [dbo].[vw_rpt_Admit_Card_Info] as
select 
	s.Id[StudentId],
	s.ClassRoll,
	s.Name[StudentName],
	s.FatherName,
	s.MotherName,
	ses.Name[SessionName],
	c.Name[ClassName],
	sec.Id[AcademicSectionId],
	sec.Name[SectionName],
	sub.SubjectCode,
	sub.SubjectName,
	e.MonthId,
	s.AcademicClassId,
	t.ExamTypeName,
	i.Name[InstituteName],
	i.EIIN,
	g.Name[Gender]
from Institute i,AcademicExamDetails d
inner join Student as s on d.StudentId = s.Id
inner join AcademicExams e on d.AcademicExamId=e.Id
inner join AcademicExamTypes as t on e.AcademicExamTypeId = t.Id
inner join AcademicSession as ses on s.AcademicSessionId = ses.Id
inner join AcademicClass c on s.AcademicClassId=c.Id
inner join AcademicSubject as sub on e.AcademicSubjectId=sub.Id
inner join AcademicSection as sec on e.AcademicSectionId = sec.Id
inner join Gender g on s.GenderId = g.Id
GO


