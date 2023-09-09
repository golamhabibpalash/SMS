USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_wise_marksheet]    Script Date: 11-Aug-23 8:49:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER       VIEW [dbo].[vw_rpt_student_wise_marksheet] 
AS
select s.Name[Student_Name],
s.FatherName,
s.MotherName,
s.DOB,
s.ClassRoll,
c.Name[ClassName], 
sec.Name[SectionName],
r.Name[ReligionName],
'Regular'[Type],
sub.SubjectName,
e.TotalMarks,
d.ObtainMark,
(select Max(det.ObtainMark)[maxNumber] from AcademicExamDetails det where det.AcademicExamId = e.Id)[MaxNumber],
(select gt.LetterGrade  from GradingTables gt where (d.ObtainMark*(100/e.TotalMarks)) between gt.NumberRangeMin and gt.NumberRangeMax)[Grade],
(select gt.gradepoint  from GradingTables gt where (d.ObtainMark*(100/e.TotalMarks)) between gt.NumberRangeMin and gt.NumberRangeMax)[Point],
g.Id[ExamGroupId],
c.Id[ClassId]
from AcademicExams e
left join student s on e.AcademicClassId = s.AcademicClassId
left join AcademicClass c on e.AcademicClassId=c.Id
left join AcademicSection sec on s.AcademicSectionId = sec.Id
left join Religion r on s.ReligionId = r.Id
left join AcademicExamGroups g on e.AcademicExamGroupId = g.Id
left join AcademicSubject sub on e.AcademicSubjectId = sub.Id
left join AcademicExamDetails d on s.Id = d.StudentId and e.Id = d.AcademicExamId
GO


