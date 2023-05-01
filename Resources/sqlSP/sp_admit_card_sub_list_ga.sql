USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[sp_admit_card_sub_list_ga]    Script Date: 01-May-23 10:17:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <21 April 2023>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_admit_card_sub_list_ga] 
	@classId int,
	@monthId int
AS
BEGIN
	select s.SubjectCode,s.SubjectName
	from AcademicSubject s
	inner join (select * from AcademicExams ex where ex.MonthId=@monthId) e on s.Id = e.AcademicSubjectId
	inner join AcademicClass c on c.Id = s.AcademicClassId
	where c.Id = @classId;
END
GO


