USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_admit_card_subject_list_gk]    Script Date: 01-May-23 10:18:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_admit_card_subject_list_gk] 
	@monthId int,
	@classRoll int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select t.SubjectCode, t.SubjectName
from AcademicSubject t
inner join (select * from AcademicExams 
where MonthId=@monthId) e on e.AcademicSubjectId = t.Id
inner join Student s on s.AcademicClassId = t.AcademicClassId
where s.ClassRoll=@classRoll and e.AcademicSectionId=s.AcademicSectionId and e.AcademicSessionId = s.AcademicSessionId;
END
GO


