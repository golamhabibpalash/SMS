USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_academic_exam_details]    Script Date: 14-Aug-23 9:41:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER VIEW [dbo].[vw_rpt_academic_exam_details]
AS
SELECT 
	s.ClassRoll,
	s.Name,
	d.ObtainMark,
	d.Status,
	d.Remarks,
	e.TotalMarks,
	d.AcademicExamId,
	d.StudentId,
(
		SELECT g.LetterGrade
		FROM GradingTables g
		WHERE ((d.ObtainMark * 100) / e.TotalMarks) BETWEEN g.NumberRangeMin
				AND g.NumberRangeMax
		) AS LetterGrade
		,(
		SELECT g.GradePoint
		FROM GradingTables g
		WHERE ((d.ObtainMark * 100) / e.TotalMarks) BETWEEN g.NumberRangeMin
				AND g.NumberRangeMax
		) AS GradePoint

FROM AcademicExamDetails d
left join Student s on d.StudentId = s.Id
LEFT JOIN AcademicExams e ON d.AcademicExamId = e.Id
GO

