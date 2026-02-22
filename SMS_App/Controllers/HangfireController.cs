using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using SMS.Entities;
using SMS_App.Utilities.EmailServices;
using SMS_App.Utilities.EmailServices.EmailVM;
using SMS_App.Utilities.LoggerService;
using SMS_App.Utilities.MACIPServices;
using SMS_App.Utilities.ShortMessageService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMS_App.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class HangfireController : ControllerBase
{
    private readonly IStudentManager _studentManager;
    private readonly IAttendanceMachineManager _attendanceMachineManager;
    private readonly IEmployeeManager _employeeManager;
    private readonly IPhoneSMSManager _phoneSMSManager;
    private readonly ISetupMobileSMSManager _setupMobileSMSManager;
    private readonly IOffDayManager _offDayManager;
    private readonly IInstituteManager _instituteManager;
    private readonly IStudentPaymentManager _studentPaymentManager;
    private readonly IParamBusConfigManager _paramBusConfigManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAppLogger _appLogger;

    public HangfireController(
        IStudentManager studentManager,
        IAttendanceMachineManager attendanceMachineManager,
        IEmployeeManager employeeManager,
        IPhoneSMSManager phoneSMSManager,
        ISetupMobileSMSManager setupMobileSMSManager,
        IOffDayManager offDayManager,
        IInstituteManager instituteManager,
        IStudentPaymentManager studentPaymentManager,
        IParamBusConfigManager paramBusConfigManager,
        IWebHostEnvironment webHostEnvironment,
        IAppLogger appLogger = null)
    {
        _studentManager = studentManager;
        _attendanceMachineManager = attendanceMachineManager;
        _employeeManager = employeeManager;
        _phoneSMSManager = phoneSMSManager;
        _setupMobileSMSManager = setupMobileSMSManager;
        _offDayManager = offDayManager;
        _instituteManager = instituteManager;
        _studentPaymentManager = studentPaymentManager;
        _paramBusConfigManager = paramBusConfigManager;
        _webHostEnvironment = webHostEnvironment;
        _appLogger = appLogger;
    }

    #region Job Scheduler ======================================================

    [HttpGet]
    public async Task<IActionResult> AttendanceBackgroundJob()
    {
        // Clear all existing jobs
        foreach (var job in JobStorage.Current.GetConnection().GetRecurringJobs())
        {
            BackgroundJob.Delete(job.Id);
            RecurringJob.RemoveIfExists(job.Id);
        }

        var institute = await _instituteManager.GetFirstOrDefaultAsync();
        int startTimeHr = institute.StartingTime.Hour;
        int startTimeMn = institute.StartingTime.Minute;
        int endTimeHr = institute.ClosingTime.Hour;

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null)
        {
            await _appLogger.WarningAsync("SMS setup not found. No jobs scheduled.");
            return RedirectToAction("SMSControl", "Setup");
        }

        var localOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local };
        if (!smsSetup.SMSService)
        {
            await _appLogger.InfoAsync("SMS Service is inactive. No jobs scheduled.");
        }

        // 1. CheckIn Summary SMS
        if (smsSetup.CheckInSMSSummary)
        {
            var (summaryHr, summaryMn) = await LoadTimeConfigAsync(7,
                defaultHour: (startTimeHr + 1).ToString(),
                defaultMinute: (startTimeMn + 5).ToString());

            RecurringJob.AddOrUpdate<HangfireController>(
                "SendCheckinSummarySMS",
                x => x.SMSSendDailyAttendanceSummary(),
                $"{summaryMn} {summaryHr} * * 0-4,6",
                localOptions);
        }

        // 2. CheckIn SMS
        if (smsSetup.CheckInSMSService)
        {
            var (checkInStartHr, _) = await LoadTimeConfigAsync(1,
                defaultHour: (startTimeHr - 1).ToString(),
                defaultMinute: "00");

            var (checkInEndHr, _) = await LoadTimeConfigAsync(2,
                defaultHour: (startTimeHr + 1).ToString(),
                defaultMinute: "00");

            string checkInCron = $"*/10 {checkInStartHr}-{checkInEndHr} * * 0-4,6";
            await _appLogger.InfoAsync($"CheckIn SMS scheduled. Cron: [{checkInCron}]");

            RecurringJob.AddOrUpdate<HangfireController>(
                "SendCheckinSMS",
                x => x.SendCheckInSMSAsync(),
                checkInCron,
                localOptions);
        }

        // 3. CheckOut SMS
        if (smsSetup.CheckOutSMSService)
        {
            var (checkOutStartHr, _) = await LoadTimeConfigAsync(3,
                defaultHour: ((startTimeHr + endTimeHr) / 2).ToString(),
                defaultMinute: "00");

            var (checkOutEndHr, _) = await LoadTimeConfigAsync(4,
                defaultHour: (endTimeHr + 1).ToString(),
                defaultMinute: "00");

            RecurringJob.AddOrUpdate<HangfireController>(
                "SendCheckoutSMS",
                x => x.SendCheckOutSMSAsync(),
                $"*/10 {checkOutStartHr}-{checkOutEndHr} * * 0-4,6",
                localOptions);
        }

        // 4. Absent Notification SMS
        if (smsSetup.AbsentNotification)
        {
            var (absentHr, absentMn) = await LoadTimeConfigAsync(8,
                defaultHour: (startTimeHr + 2).ToString(),
                defaultMinute: "1");

            RecurringJob.AddOrUpdate<HangfireController>(
                "AbsentNotificationSMS",
                x => x.SendAbsentNotificationSMSAsync(),
                $"{absentMn} {absentHr} * * 0-4,6",
                localOptions);
        }

        // 5. Daily Collection SMS
        if (smsSetup.DailyCollectionSMSService)
        {
            var (collectionHr, collectionMn) = await LoadTimeConfigAsync(9,
                defaultHour: "18",
                defaultMinute: "1");

            RecurringJob.AddOrUpdate<HangfireController>(
                "DailyCollectionSummerySMS",
                x => x.SendDailyCollectionSMSAsync(),
                $"{collectionMn} {collectionHr} * * 0-4,6",
                localOptions);
        }

        return RedirectToAction("SMSControl", "Setup");
    }

    /// <summary>
    /// Loads time config from DB by paramSL and parses it safely.
    /// </summary>
    private async Task<(string Hour, string Minute)> LoadTimeConfigAsync(
        int paramSL, string defaultHour, string defaultMinute)
    {
        var config = await _paramBusConfigManager.GetByParamSL(paramSL);
        return ParseTimeConfig(config?.ParamValue, defaultHour, defaultMinute);
    }

    private static (string Hour, string Minute) ParseTimeConfig(
        string paramValue, string defaultHour, string defaultMinute)
    {
        if (string.IsNullOrWhiteSpace(paramValue) || !paramValue.Contains(':'))
            return (defaultHour, defaultMinute);

        int idx = paramValue.IndexOf(':');
        string hour = paramValue[..idx].Trim();
        string minute = paramValue[(idx + 1)..].Trim();

        if (!int.TryParse(hour, out int h) || h < 0 || h > 23) hour = defaultHour;
        if (!int.TryParse(minute, out int m) || m < 0 || m > 59) minute = defaultMinute;

        return (hour, minute);
    }

    #endregion

    #region Holiday Check ======================================================

    private async Task<bool> IsTodayHolidayAsync()
    {
        var holidays = await _offDayManager
            .GetMonthlyHolidaysAsync(DateTime.Today.ToString("MMyyyy"));

        if (holidays == null || holidays.Count == 0)
            return false;

        string today = DateTime.Today.ToString("ddMMyyyy");
        return holidays.Any(h => h.ToString("ddMMyyyy") == today);
    }

    #endregion

    #region CheckIn SMS ========================================================

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task SendCheckInSMSAsync()
    {
        if (await IsTodayHolidayAsync())
        {
            await _appLogger.InfoAsync("Today is a holiday. CheckIn SMS skipped.");
            return;
        }

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null || !smsSetup.SMSService || !smsSetup.CheckInSMSService)
        {
            await _appLogger.InfoAsync("CheckIn SMS Service is inactive or setup missing.");
            return;
        }

        var todaysAttendance = await _attendanceMachineManager
            .GetAllAttendanceByDateAsync(DateTime.Today);

        if (todaysAttendance == null || todaysAttendance.Count == 0)
        {
            await _appLogger.InfoAsync("No attendance records found for today.");
            return;
        }

        var allActiveStudents = await _studentManager.GetAllActiveStudentsAsync();

        if (smsSetup.CheckInSMSServiceForEmployees)
            await ProcessCheckInForEmployeesAsync(todaysAttendance);

        if (smsSetup.CheckInSMSServiceForMaleStudent)
            await ProcessAttendanceForGenderAsync(todaysAttendance, allActiveStudents, GenderType.Male, "CheckIn");

        if (smsSetup.CheckInSMSServiceForGirlsStudent)
            await ProcessAttendanceForGenderAsync(todaysAttendance, allActiveStudents, GenderType.Female, "CheckIn");

        await _appLogger.InfoAsync("CheckIn SMS processing completed.");
    }

    private async Task ProcessCheckInForEmployeesAsync(IList<Tran_MachineRawPunch> attendanceRecords)
    {
        const string smsType = "CheckIn";
        string today = DateTime.Today.ToString("dd-MM-yyyy");

        foreach (var att in attendanceRecords)
        {
            try
            {
                if (!int.TryParse(att.CardNo?.Trim(), out int empId))
                    continue;

                var employee = await _employeeManager.GetByIdAsync(empId);
                if (!IsEligibleEmployee(employee))
                    continue;

                string phoneNumber = employee.Phone;
                if (string.IsNullOrEmpty(phoneNumber) || !PhoneNumberValidate(phoneNumber))
                    continue;

                if (await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, today))
                    continue;

                string name = GetEmployeeName(employee);
                string smsText = GenerateCheckInSMSText(name, att.PunchDatetime.ToString("hh:mm tt"));

                if (await MobileSMS.SendSMS(phoneNumber, smsText))
                    await SaveSMSRecordAsync(phoneNumber, smsText, smsType);
            }
            catch (Exception ex)
            {
                await _appLogger.ErrorAsync(
                    $"Error processing CheckIn SMS for Employee CardNo: {att.CardNo}", ex.Message);
            }
        }
    }

    #endregion

    #region CheckOut SMS =======================================================

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task SendCheckOutSMSAsync()
    {
        if (await IsTodayHolidayAsync())
        {
            await _appLogger.InfoAsync("Today is a holiday. CheckOut SMS skipped.");
            return;
        }

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null || !smsSetup.CheckOutSMSService)
        {
            await _appLogger.InfoAsync("CheckOut SMS Service is inactive.");
            return;
        }

        var todaysCheckOut = await _attendanceMachineManager
            .GetCheckOutDataByDateAsync(DateTime.Today.ToString("dd-MM-yyyy"));

        if (todaysCheckOut == null || todaysCheckOut.Count == 0)
        {
            await _appLogger.InfoAsync("No checkout records found for today.");
            return;
        }

        var allActiveStudents = await _studentManager.GetAllActiveStudentsAsync();

        if (smsSetup.CheckOutSMSServiceForEmployees)
            await ProcessCheckOutForEmployeesAsync(todaysCheckOut);

        if (smsSetup.CheckOutSMSServiceForMaleStudent)
            await ProcessAttendanceForGenderAsync(todaysCheckOut, allActiveStudents, GenderType.Male, "CheckOut");

        if (smsSetup.CheckOutSMSServiceForGirlsStudent)
            await ProcessAttendanceForGenderAsync(todaysCheckOut, allActiveStudents, GenderType.Female, "CheckOut");

        await _appLogger.InfoAsync("CheckOut SMS processing completed.");
    }

    private async Task ProcessCheckOutForEmployeesAsync(IList<Tran_MachineRawPunch> attendanceRecords)
    {
        const string smsType = "CheckOut";
        string today = DateTime.Today.ToString("dd-MM-yyyy");

        foreach (var att in attendanceRecords)
        {
            try
            {
                if (!int.TryParse(att.CardNo?.Trim(), out int empId))
                    continue;

                var employee = await _employeeManager.GetByIdAsync(empId);
                if (!IsEligibleEmployee(employee))
                    continue;

                string phoneNumber = employee.Phone;
                if (string.IsNullOrEmpty(phoneNumber) || !PhoneNumberValidate(phoneNumber))
                    continue;

                if (await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, today))
                    continue;

                string name = GetEmployeeName(employee);
                string smsText = GenerateCheckOutSMSText(name, att.PunchDatetime.ToString("hh:mm tt"));

                if (await MobileSMS.SendSMS(phoneNumber, smsText))
                    await SaveSMSRecordAsync(phoneNumber, smsText, smsType);
            }
            catch (Exception ex)
            {
                await _appLogger.ErrorAsync(
                    $"Error processing CheckOut SMS for Employee CardNo: {att.CardNo}", ex.Message);
            }
        }
    }

    #endregion

    #region Shared Student Attendance Processing ================================

    private async Task ProcessAttendanceForGenderAsync(
        IList<Tran_MachineRawPunch> attendanceRecords,
        IEnumerable<Student> allActiveStudents,
        GenderType gender,
        string smsType)
    {
        var studentLookup = allActiveStudents
            .Where(s => s.Status == true && s.SMSService == true && s.GenderId == (int)gender)
            .ToDictionary(s => s.UniqueId.Trim(), s => s);

        foreach (var attendance in attendanceRecords)
            await ProcessSingleStudentSMSAsync(attendance, studentLookup, smsType);
    }

    private async Task ProcessSingleStudentSMSAsync(
        Tran_MachineRawPunch attendance,
        Dictionary<string, Student> studentLookup,
        string smsType)
    {
        try
        {
            string cardNo = attendance.CardNo?.Trim();
            if (string.IsNullOrEmpty(cardNo) || !studentLookup.TryGetValue(cardNo, out var student))
                return;

            string phoneNumber = GetStudentPhoneNumber(student);
            if (string.IsNullOrEmpty(phoneNumber) || !PhoneNumberValidate(phoneNumber))
                return;

            string today = DateTime.Today.ToString("dd-MM-yyyy");
            if (await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, today))
                return;

            string name = GetStudentName(student);
            string time = attendance.PunchDatetime.ToString("hh:mm tt");
            string smsText = smsType == "CheckIn"
                ? GenerateCheckInSMSText(name, time)
                : GenerateCheckOutSMSText(name, time);

            if (await MobileSMS.SendSMS(phoneNumber, smsText))
                await SaveSMSRecordAsync(phoneNumber, smsText, smsType);
        }
        catch (Exception ex)
        {
            await _appLogger.ErrorAsync(
                $"Error processing {smsType} SMS for CardNo: {attendance.CardNo}", ex.Message);
        }
    }

    #endregion

    #region Absent Notification SMS ============================================

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task SendAbsentNotificationSMSAsync()
    {
        if (await IsTodayHolidayAsync())
        {
            await _appLogger.InfoAsync("Today is a holiday. Absent notification skipped.");
            return;
        }

        string today = DateTime.Today.ToString("dd-MM-yyyy");
        var allCheckIn = await _attendanceMachineManager.GetCheckinDataByDateAsync(today);

        // Only send absent notifications if machine has meaningful data (>10 records)
        if (allCheckIn == null || allCheckIn.Count <= 10)
        {
            await _appLogger.InfoAsync("Insufficient check-in data. Absent notification skipped.");
            return;
        }

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null || !smsSetup.AbsentNotification)
            return;

        if (smsSetup.AbsentNotificationStudent)
            await ProcessAbsentStudentSMSAsync(today);

        if (smsSetup.AbsentNotificationEmployee)
            await ProcessAbsentEmployeeSMSAsync(today);
    }

    private async Task ProcessAbsentStudentSMSAsync(string date)
    {
        var absentStudents = await _attendanceMachineManager.GetTodaysAbsentStudentAsync(date);
        if (absentStudents == null || absentStudents.Count == 0)
            return;

        const string smsType = "absent";

        foreach (var student in absentStudents)
        {
            try
            {
                if (student.Status == false) continue;

                string phoneNumber = GetStudentPhoneNumber(student);
                if (string.IsNullOrEmpty(phoneNumber)) continue;

                if (await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, date))
                    continue;

                string name = GetStudentName(student);
                string smsText = GenerateAbsentNotificationText(name, "student", 1);

                if (await MobileSMS.SendSMS(phoneNumber, smsText))
                    await SaveSMSRecordAsync(phoneNumber, smsText, smsType);
            }
            catch (Exception ex)
            {
                await _appLogger.ErrorAsync(
                    $"Error sending absent SMS for student: {student.Name}", ex.Message);
            }
        }
    }

    private async Task ProcessAbsentEmployeeSMSAsync(string date)
    {
        var absentEmployees = await _attendanceMachineManager.GetTodaysAbsentEmployeeAsync(date);
        if (absentEmployees == null || absentEmployees.Count == 0)
            return;

        const string smsType = "absent";

        foreach (var employee in absentEmployees)
        {
            try
            {
                if (employee.Status != true) continue;

                string phoneNumber = employee.Phone;
                if (string.IsNullOrEmpty(phoneNumber)) continue;

                if (await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, date))
                    continue;

                string name = GetEmployeeName(employee);
                string smsText = GenerateAbsentNotificationText(name, "employee", 1);

                if (await MobileSMS.SendSMS(phoneNumber, smsText))
                    await SaveSMSRecordAsync(phoneNumber, smsText, smsType);
            }
            catch (Exception ex)
            {
                await _appLogger.ErrorAsync(
                    $"Error sending absent SMS for employee: {employee.EmployeeName}", ex.Message);
            }
        }
    }

    #endregion

    #region Summary SMS ========================================================

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task SMSSendDailyAttendanceSummary()
    {
        if (await IsTodayHolidayAsync())
        {
            await _appLogger.InfoAsync("Today is a holiday. Summary SMS skipped.");
            return;
        }

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null || !smsSetup.CheckInSMSSummary)
            return;

        string today = DateTime.Today.ToString("dd-MM-yyyy");
        var allCheckIn = await _attendanceMachineManager.GetCheckinDataByDateAsync(today);
        if (allCheckIn == null || !allCheckIn.Any())
            return;

        var students = await _studentManager.GetAllAsync();
        var employees = await _employeeManager.GetAllAsync();
        var institute = (await _instituteManager.GetAllAsync()).FirstOrDefault();

        int boysCount = CountAttendance(allCheckIn, students, GenderType.Male);
        int girlsCount = CountAttendance(allCheckIn, students, GenderType.Female);
        int totalStudents = boysCount + girlsCount;

        var paddedEmployeeIds = employees
            .Select(e => e.Id.ToString().PadLeft(8, '0'))
            .ToHashSet();
        int totalEmployees = allCheckIn.Count(a => paddedEmployeeIds.Contains(a.CardNo.Trim()));

        string summaryMsg = totalStudents <= 0
            ? "Your attendance machine is off or disconnected!"
            : $"Attendance Summary ({DateTime.Today:dd MMM yyyy}):\n" +
              $"Employees: {totalEmployees}\n" +
              $"Students: ({boysCount}+{girlsCount})= {totalStudents}\n" +
              $"-{institute?.ShortName}";

        // Send Email
        string toEmailString = await _paramBusConfigManager.GetValueByParamSL(11);
        if (!string.IsNullOrEmpty(toEmailString))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Institute");
            var summaryVM = new AttendanceSummary
            {
                BackgroundImageUrl = Path.Combine(filePath, institute?.Logo ?? ""),
                InstituteName = institute?.Name,
                AttendanceDate = DateTime.Today.ToString("dd MMM yyyy"),
                BoysCount = boysCount.ToString(),
                GirlsCount = girlsCount.ToString(),
                TotalCount = totalStudents.ToString(),
                EmployeesCount = totalEmployees.ToString()
            };

            foreach (var email in toEmailString.Split(','))
                EmailService.SendAttendanceEmail(
                    email.Trim(),
                    $"Today's ({DateTime.Today:dd MMM yyyy}) attendance summary",
                    summaryVM);
        }

        // Send SMS
        string phoneNumberString = await _paramBusConfigManager.GetValueByParamSL(10);
        if (!string.IsNullOrEmpty(phoneNumberString))
        {
            const string smsType = "CheckIn Summary";
            foreach (var num in phoneNumberString.Split(','))
            {
                string trimmedNum = num.Trim();
                if (await _phoneSMSManager.IsSMSSendForAttendance(trimmedNum, smsType, DateTime.Today.ToString("dd-MM-yyyy")))
                    continue;

                if (await MobileSMS.SendSMS(trimmedNum, summaryMsg))
                    await SaveSMSRecordAsync(trimmedNum, summaryMsg, smsType);
            }
        }

        await _appLogger.InfoAsync("Attendance summary SMS completed.");
    }

    private static int CountAttendance(
        IEnumerable<Tran_MachineRawPunch> attendance,
        IEnumerable<Student> students,
        GenderType gender)
    {
        var studentIds = students
            .Where(s => s.Status == true && s.GenderId == (int)gender)
            .Select(s => s.UniqueId.Trim())
            .ToHashSet();

        return attendance.Count(a => studentIds.Contains(a.CardNo.Trim()));
    }

    #endregion

    #region Daily Collection SMS ===============================================

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public async Task SendDailyCollectionSMSAsync()
    {
        if (await IsTodayHolidayAsync())
        {
            await _appLogger.InfoAsync("Today is a holiday. Collection SMS skipped.");
            return;
        }

        var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
        if (smsSetup == null || !smsSetup.DailyCollectionSMSService)
            return;

        var payments = await _studentPaymentManager.GetStudentPaymentSummerySMS_VMsAsync(DateTime.Today);
        var institute = (await _instituteManager.GetAllAsync()).FirstOrDefault();

        if (payments == null || !payments.Any())
            return;

        var summary = payments.FirstOrDefault();
        string phoneNumberString = await _paramBusConfigManager.GetValueByParamSL(12);
        if (string.IsNullOrEmpty(phoneNumberString))
            return;

        const string smsType = "Collection_sum";
        string smsText =
            $"Payment Collection ({DateTime.Today:dd MMM yyyy}):\n" +
            $"Residential: {summary.ResidentialPayment}\n" +
            $"Non-Residential: {summary.NonResidentialPayment}\n" +
            $"Total = {summary.ResidentialPayment + summary.NonResidentialPayment}\n" +
            $"-{institute?.Name}";

        foreach (var num in phoneNumberString.Split(','))
        {
            string trimmedNum = num.Trim();
            if (await _phoneSMSManager.IsSMSSendForAttendance(trimmedNum, smsType, DateTime.Today.ToString("dd-MM-yyyy")))
                continue;

            if (await MobileSMS.SendSMS(trimmedNum, smsText))
                await SaveSMSRecordAsync(trimmedNum, smsText, smsType);
        }

        await _appLogger.InfoAsync("Daily collection SMS completed.");
    }

    #endregion

    #region Shared Helpers =====================================================

    private async Task SaveSMSRecordAsync(string phoneNumber, string smsText, string smsType)
    {
        await _phoneSMSManager.AddAsync(new PhoneSMS
        {
            Text = smsText,
            MobileNumber = phoneNumber,
            SMSType = smsType,
            CreatedBy = "Automation",
            CreatedAt = DateTime.Now,
            EditedBy = "Automation",
            EditedAt = DateTime.Now,
            MACAddress = MACService.GetMAC()
        });
    }

    private static string GetStudentPhoneNumber(Student student) =>
        !string.IsNullOrEmpty(student.GuardianPhone)
            ? student.GuardianPhone
            : student.PhoneNo ?? string.Empty;

    private static string GetStudentName(Student student) =>
        !string.IsNullOrEmpty(student.NameBangla) ? student.NameBangla : student.Name;

    private static string GetEmployeeName(Employee employee) =>
        !string.IsNullOrEmpty(employee.EmployeeNameBangla)
            ? employee.EmployeeNameBangla
            : employee.EmployeeName;

    private static bool IsEligibleEmployee(Employee employee) =>
        employee != null && employee.Status == true;

    private bool PhoneNumberValidate(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        if (!long.TryParse(phoneNumber.Trim(), out long pNumber)) return false;
        return pNumber >= 01300000000L && pNumber <= 01999999999L;
    }

    public enum GenderType { Male = 1, Female = 2 }

    #endregion

    #region SMS Text Generators ================================================

    private string GenerateCheckInSMSText(string name, string time) =>
        $"{name} আজ {time} মিনিটে স্কুলে উপস্থিত হয়েছে। -নোবেল ।";

    private string GenerateCheckOutSMSText(string name, string time) =>
        $"{name} স্কুল থেকে {time} মিনিটে প্রস্থান করেছে। -নোবেল ।";

    private string GenerateAbsentNotificationText(string name, string smsFor, int absentDayCount)
    {
        if (absentDayCount == 1)
            return smsFor == "employee"
                ? $"{name} is not in school today."
                : $"{name} আজ ({DateTime.Now:dd MMM yyyy}) স্কুলে আসেনি । -নোবেল।";

        return $"{name} গত {absentDayCount} দিন থেকে স্কুলে আসছে না । -নোবেল।";
    }

    #endregion
}