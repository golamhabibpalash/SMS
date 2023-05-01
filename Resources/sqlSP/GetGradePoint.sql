USE [SMSDB]
GO

/****** Object:  StoredProcedure [dbo].[GetGradePoint]    Script Date: 01-May-23 10:19:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER   PROCEDURE [dbo].[GetGradePoint] 
@Score DECIMAL(5,2), 
@TotalMark int
AS
BEGIN
    DECLARE @GradePoint DECIMAL(5,2)

    IF (@Score >= ((80*@TotalMark)/100) AND @Score <= @TotalMark) SET @GradePoint = 5.00
    ELSE IF (@Score >= ((70*@TotalMark)/100) AND @Score <= ((79*@TotalMark)/100)) SET @GradePoint = 4.00
    ELSE IF (@Score >= ((60*@TotalMark)/100) AND @Score <= ((69*@TotalMark)/100)) SET @GradePoint = 3.5
    ELSE IF (@Score >= ((50*@TotalMark)/100) AND @Score <= ((59*@TotalMark)/100)) SET @GradePoint = 3.00
    ELSE IF (@Score >= ((40*@TotalMark)/100) AND @Score <= ((49*@TotalMark)/100)) SET @GradePoint = 2.00
    ELSE IF (@Score >= ((33*@TotalMark)/100) AND @Score <= ((39*@TotalMark)/100)) SET @GradePoint = 1.00
    ELSE SET @GradePoint = 0

    SELECT @GradePoint AS 'Grade Point'
END
GO


