USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_wise_marksheet]    Script Date: 30-Sep-23 6:45:41 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE OR ALTER         VIEW [dbo].[vw_rpt_student_wise_marksheet] 
AS
select 
gr.ExamGroupName,
c.Name[ClassName],
s.Name[StudentName],
s.FatherName[FatherName],
s.MotherName,
s.ClassRoll,
sec.Name[SectionName],
sec.Id[AcademicSectionId],
g.Name[GenderName],
sub.SubjectName,
d.TotalMark,
d.ObtainMark,
d.GPA,
d.Grade,
(select max(erd.obtainMark) from ExamResultDetails erd where erd.AcademicSubjectId = sub.Id and erd.ExamResultId=r.Id group by erd.ObtainMark)[MaxNumber],
r.CGPA[FinalGPA],
r.FinalGrade,
r.AttendancePercentage,
r.TotalObtainMarks,
r.TotalFails,
r.Rank[MeritPosition],
r.GradeComments,
gr.Id[ExamGroupId],
r.AcademicClassId,
r.StudentId,
s.DOB,
reli.Name[ReligionName],
r.CreatedAt
from ExamResults r
left join ExamResultDetails d on r.Id=d.ExamResultId
left join AcademicSubject sub on d.AcademicSubjectId = sub.Id
left join Student s on r.StudentId=s.Id
left join AcademicClass c on r.AcademicClassId = c.Id
left join AcademicSection sec on s.AcademicSectionId = sec.Id
left join Gender g on s.GenderId = g.Id
left join Religion reli on s.ReligionId = reli.Id
left join AcademicExamGroups gr on r.AcademicExamGroupId = gr.Id
GO


