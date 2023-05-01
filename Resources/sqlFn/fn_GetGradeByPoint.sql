USE [SMSDB]
GO

/****** Object:  UserDefinedFunction [dbo].[fn_GetGradeByPoint]    Script Date: 01-May-23 10:03:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[fn_GetGradeByPoint] 
(
 @GradePoint DECIMAL(5,2)
)
RETURNS varchar(2)
AS
BEGIN
	DECLARE @Grade varchar(2)

	IF(@GradePoint<=5.00 and @GradePoint>4.99) SET @Grade='A+'
	ELSE IF(@GradePoint<=4.99 and @GradePoint>=4.00) SET @Grade='A'
	ELSE IF(@GradePoint<=3.99 and @GradePoint>=3.50) SET @Grade='A-'
	ELSE IF(@GradePoint<=3.49 and @GradePoint>=3.00) SET @Grade='B'
	ELSE IF(@GradePoint<=2.99 and @GradePoint>=2.00) SET @Grade='C'
	ELSE IF(@GradePoint<=1.99 and @GradePoint>=1.00) SET @Grade='D'
	ELSE SET @Grade='F'

	RETURN @Grade
END
GO


