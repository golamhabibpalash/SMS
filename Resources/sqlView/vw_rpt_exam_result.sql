USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_exam_result]    Script Date: 01-May-23 10:02:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   view [dbo].[vw_rpt_exam_result]
as 
select
ae.ExamName,

CAST(stu.ClassRoll as varchar)[ClassRoll],
stu.Name[StudentName],
sub.SubjectName,
ses.Name[AcademicSessionName],
c.Name[AcademicClassName],
et.ExamTypeName,
CAST(aed.ObtainMark as varchar)[ObtainMark],
CAST((SELECT dbo.fn_GetGradePoint (aed.ObtainMark, ae.TotalMarks))as varchar)[Point],
(select dbo.fn_GetGradeByPoint(dbo.fn_GetGradePoint (aed.ObtainMark, ae.TotalMarks)))[Grade],
CAST(ae.TotalMarks as varchar)[Marks],
CAST((select SUM(ed.ObtainMark)[TotalMark]from AcademicExamDetails ed
inner join Student s on ed.StudentId=s.Id
inner join AcademicExams e on ed.AcademicExamId=e.Id
where e.MonthId=ae.MonthId and s.ClassRoll=stu.ClassRoll
group by s.ClassRoll) as varchar)[TotalMarks],
CAST((select MAX(ed.ObtainMark) from AcademicExamDetails ed
inner join AcademicExams e on ed.AcademicExamId=e.Id
where e.Id=ae.Id) as varchar)[HighestNumber],
''[GradePoint],
''[CGPA],
''[Rank],
''[Status],
''[NumberOfFail],
''[PreMaritPosition],
ae.MonthId,
ae.AcademicSectionId,
ae.AcademicSessionId,
c.Id[AcademicClassId],
ae.AcademicExamTypeId,
ae.Id[AcademicExamId],
stu.Id[StudentId]
from AcademicExamDetails as aed
inner join Student as stu on aed.StudentId=stu.Id
inner join AcademicClass as c on stu.AcademicClassId = c.Id
inner join AcademicExams as ae on aed.AcademicExamId=ae.Id
inner join AcademicExamTypes as et on ae.AcademicExamTypeId=et.Id
inner join AcademicSession as ses on ae.AcademicSessionId = ses.Id
inner join AcademicSubject as sub on ae.AcademicSubjectId=sub.Id
GO


