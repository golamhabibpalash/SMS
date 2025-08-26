CREATE PROCEDURE sp_GetCheckOutReportData
    @ReportDate DATE,
    @ClassId INT = NULL,
    @SectionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CheckOutStart TIME;
    DECLARE @CheckOutEnd TIME;

    -- Get CheckOut times from ParamBusConfigs
    SELECT @CheckOutStart = ParamValue FROM ParamBusConfigs WHERE ParamSL = 3;
    SELECT @CheckOutEnd   = ParamValue FROM ParamBusConfigs WHERE ParamSL = 4;

    -- Core query
    SELECT 
        c.Name AS ClassOrDesignationName,
        s.ClassRoll AS RollOrCard,
        s.Name,
        s.PhoneNo AS Phone,
        s.GuardianPhone AS AlternativePhone,
        RIGHT(CONVERT(VARCHAR(20), p.PunchDatetime, 100), 7) AS CheckOut,
        RIGHT(CONVERT(VARCHAR(20), sms.CreatedAt, 100), 7) AS SMSSent
    FROM Student s
    LEFT JOIN AcademicClass c ON s.AcademicClassId = c.Id
    CROSS APPLY (
        SELECT TOP 1 *
        FROM Tran_MachineRawPunch t
        WHERE t.CardNo = s.UniqueId
          AND t.PunchDatetime >= @ReportDate
          AND t.PunchDatetime < DATEADD(DAY, 1, @ReportDate)
          AND CONVERT(TIME, t.PunchDatetime) BETWEEN @CheckOutStart AND @CheckOutEnd
        ORDER BY t.PunchDatetime DESC
    ) p
    LEFT JOIN (
        SELECT *
        FROM PhoneSMS
        WHERE SMSType = 'CheckOut'
          AND CreatedAt >= @ReportDate
          AND CreatedAt < DATEADD(DAY, 1, @ReportDate)
    ) sms ON s.GuardianPhone = sms.MobileNumber
    WHERE (@ClassId IS NULL OR s.AcademicClassId = @ClassId)
      AND (@SectionId IS NULL OR s.AcademicSectionId = @SectionId)
    ORDER BY s.ClassRoll;
END;



