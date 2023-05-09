USE [SMSDB]
GO

/****** Object:  View [dbo].[vw_rpt_student_info]    Script Date: 06-May-23 12:00:14 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER VIEW [dbo].[vw_rpt_student_info] 
AS
SELECT s.ClassRoll, s.Name 'StudentName', c.Name 'ClassName', c.Id[AcademicClassId], c.ClassSerial, acSession.Name 'SessionName', acSession.Id 'academicSessionId', aSec.Name 'SectionName', aSec.Id 'academicSectionId', s.FatherName, s.MotherName, s.GuardianPhone, s.PhoneNo, g.Name 'Gender', r.Name 'Religion', b.Name 'BloodGroup', s.Status 'Status' FROM Student s 
LEFT JOIN academicClass c ON s.AcademicClassId = c.Id 
LEFT JOIN academicSession acSession ON s.AcademicSessionId = acSession.Id
LEFT JOIN Gender g ON s.GenderId = g.Id 
LEFT JOIN Religion r ON s.religionId = r.Id 
LEFT JOIN AcademicSection aSec ON s.AcademicSectionId = aSec.Id 
LEFT JOIN BloodGroup b ON s.BloodGroupId = b.Id
ORDER BY c.ClassSerial, s.ClassRoll offset 0 rows
GO


