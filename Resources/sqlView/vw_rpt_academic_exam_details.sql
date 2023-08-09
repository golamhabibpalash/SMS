USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_academic_exam_details]    Script Date: 05-Aug-23 10:43:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER   VIEW [dbo].[vw_rpt_academic_exam_details]
AS
select
s.ClassRoll,
s.Name,
d.ObtainMark,
d.Status,
d.Remarks,
e.TotalMarks,
d.AcademicExamId,
d.StudentId as StudentId,
(select t.LetterGrade from GradingTables t where ((d.ObtainMark*100)/e.TotalMarks)>= t.NumberRangeMin and ((d.ObtainMark*100)/e.TotalMarks)<= t.NumberRangeMax ) as LetterGrade,
(select t.GradePoint from GradingTables t where ((d.ObtainMark*100)/e.TotalMarks)>= t.NumberRangeMin and ((d.ObtainMark*100)/e.TotalMarks)<= t.NumberRangeMax ) as GradePoint
from AcademicExamDetails d
inner join Student s on d.StudentId = s.Id
inner join AcademicExams e on d.AcademicExamId = e.Id;
GO

