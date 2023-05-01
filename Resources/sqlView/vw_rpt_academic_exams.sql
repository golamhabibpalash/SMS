USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_academic_exams]    Script Date: 01-May-23 10:00:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER view [dbo].[vw_rpt_academic_exams] as
select 
	s.id[StudentId],
	s.ClassRoll,
	s.Name[StudentName],
	s.FatherName,
	s.MotherName,
	sec.Name[SectionName],
	cl.Name[ClassName],
	e.Id[AcademicExamId],
	e.AcademicSubjectId,
	e.AcademicSectionId,
	e.MonthId,
	e.ExamName,
	sub.SubjectCode,
	sub.SubjectName,
	sub.AcademicClassId
from Student as s
left join AcademicSection as sec on s.AcademicSectionId = sec.Id
left join academicClass as cl on s.academicClassId = cl.Id
inner join AcademicExamDetails as t on s.Id=t.StudentId
left join AcademicExams as e on t.AcademicExamId= e.Id
left join AcademicSubject as sub on e.AcademicSubjectId = sub.Id
GO


