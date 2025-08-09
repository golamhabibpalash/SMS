using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using SchoolManagementSystem;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SMS.App.Utilities.Others;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.ReportVM;
using SMS.App.ViewModels.ReportVM.MarkSheet;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Reports;
using SMS.Entities;
using SMS.Entities.RptModels.AttendanceVM;
using SMS.Entities.RptModels.StudentPayment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Image = System.Drawing.Image;
using LocalReport = Microsoft.Reporting.NETCore.LocalReport;

namespace SMS.App.Controllers;

[Authorize]
public class ReportsController : Controller
{
    #region properties
    private readonly IWebHostEnvironment _host;
    private readonly IStudentManager _studentManager;
    private readonly IReportManager _reportManager;
    private readonly IAcademicClassManager _academicClassManager;
    private readonly IAttendanceMachineManager _attendanceMachineManager;
    private readonly IOffDayManager _OffDayManager;
    private readonly IAcademicSessionManager _academicSessionManager;
    private readonly IAcademicSectionManager _academicSectionManager;
    private readonly IInstituteManager _instituteManager;
    private readonly IAcademicExamDetailsManager _academicExamDetailsManager;
    private readonly IAcademicExamManager _academicExamManager;
    private readonly IGradingTableManager _gradingTableManager;
    private readonly IAcademicExamGroupManager _academicExamGroupManager;
    private readonly IStudentFeeHeadManager _studentFeeHeadManager;
    private readonly IExamResultManager _examResultManager;

    #endregion properties

    #region Constructor
    public ReportsController(IWebHostEnvironment host, IStudentManager studentManager, IReportManager reportManager, IAcademicClassManager academicClassManager, IAttendanceMachineManager attendanceMachineManager, IOffDayManager dayManager, IAcademicSessionManager academicSessionManager, IAcademicSectionManager academicSectionManager, IInstituteManager instituteManager, IAcademicExamDetailsManager academicExamDetailsManager, IAcademicExamManager academicExamManager, IGradingTableManager gradingTableManager, IAcademicExamGroupManager academicExamGroupManager, IStudentFeeHeadManager studentFeeHeadManager, IExamResultManager examResultManager)
    {
        _host = host;
        _studentManager = studentManager;
        _reportManager = reportManager;
        _academicClassManager = academicClassManager;
        _attendanceMachineManager = attendanceMachineManager;
        _OffDayManager = dayManager;
        _academicSessionManager = academicSessionManager;
        _academicSectionManager = academicSectionManager;
        _instituteManager = instituteManager;
        _academicExamDetailsManager = academicExamDetailsManager;
        _academicExamManager = academicExamManager;
        _gradingTableManager = gradingTableManager;
        _academicExamGroupManager = academicExamGroupManager;
        _studentFeeHeadManager = studentFeeHeadManager;
        _examResultManager = examResultManager;
    }
    #endregion Constructor

    #region Student List Report
    [Authorize(Policy = "StudentsReportsPolicy")]
    public async Task<IActionResult> StudentsReport()
    {
        Rpt_Student_VM rpt_Student_VM = new()
        {
            AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList(),
            AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name").ToList()
        };
        return View(rpt_Student_VM);
    }

    [Authorize(Policy = "StudentsReportsPolicy")]
    public async Task<IActionResult> StudentsReportExport(string reportType, string fileName, int? academicClassId, int? academicSectionId)
    {
        AcademicSession aSession = await _academicSessionManager.GetCurrentAcademicSession();
        if (aSession == null)
        {
            return new JsonResult("Current Session not set");
        }

        var studens = await _reportManager.GetStudentsInfo(aSession.Id, academicClassId, academicSectionId);
        if (studens == null || studens.Count <= 0)
        {
            return new JsonResult("Sorry! Students Not Found.");
        }
        Institute institute = await _instituteManager.GetByIdAsync(1);
        if (institute == null)
        {
            return new JsonResult("Sorry! Institute Information Not Found");
        }

        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\rptStudent.rdlc";

        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }

        using var report = new Microsoft.Reporting.NETCore.LocalReport();
        report.DataSources.Add(new ReportDataSource("DataSet1", studens));
        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("ReportName", "Student List"),
            new ReportParameter("Address", institute.Address),
            new ReportParameter("EIIN", institute.EIIN),
            new ReportParameter("Logo", imageParam)
        };
        report.ReportPath = path;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                pdf = report.Render("excel");
            }
            if (reportType == "word")
            {
                pdf = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd");
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, mediaType);
    }

    public IActionResult StudentDynamicReport()
    {
        var columnMap = new Dictionary<string, string>
        {
            { "Name", "Name" },
            { "ClassRoll", "Class Roll" },
            { "NameBangla", "Name (Bangla)" },
            { "AcademicSection", "Section" },
            { "FatherName", "Father's Name" },
            { "MotherName", "Mother's Name" },
            { "AdmissionDate", "Admission Date" },
            { "Email", "Email" },
            { "Gender", "Gender" },
            { "PhoneNo", "Phone" },
            { "GuardianPhone", "Guardian Phone" },
            { "DOB", "Date of Birth" },
            { "Religion", "Religion" },
            { "BloodGroup", "Blood Group" },
            { "PresentAddress", "Present Address" },
            { "PermanentAddress", "Permanent Address" },
            { "PreviousSchool", "Previous School" },
            { "IsResidential", "Is Residential" },
            { "SMSService", "SMS Service?" },
            { "Status", "Current Status" },
        };

        var reportModel = new RptStudentDynamicReportVM
        {
            AcademicClassList = new SelectList(_academicClassManager.GetAllAsync().Result, "Id", "Name").ToList(),
            AcademicSessionList = new SelectList(_academicSessionManager.GetAllAsync().Result, "Id", "Name").ToList(),

            // Set the ColumnMap dictionary
            ColumnMap = columnMap
        };

        return View(reportModel);
    }

    public async Task<IActionResult> StudentDynamicReportExport(string reportType, string fileName, int academicSessionId, int academicClassId, int academicSectionId, [FromQuery(Name = "columns")] List<string> columns)
    {
        if (columns == null || columns.Count == 0)
            return BadRequest("No columns selected.");

        var students = await _studentManager.GetStudentsByClassSessionSectionAsync(academicSessionId, academicClassId, academicSectionId);
        if (students == null || students.Count == 0)
            return Json("Sorry! Students Not Found.");

        var institute = await _instituteManager.GetByIdAsync(1);
        if (institute == null)
            return Json("Sorry! Institute Information Not Found");

        var columnMappings = new Dictionary<string, (string Label, Func<Student, object> Selector)>
        {
            { "Name", ("Name", s => s.Name) },
            { "ClassRoll", ("Class Roll", s => s.ClassRoll) },
            { "NameBangla", ("Name (Bangla)", s => s.NameBangla) },
            { "AcademicClass", ("Class", s => s.AcademicClass?.Name) },
            { "AcademicSection", ("Section", s => s.AcademicSection?.Name) },
            { "FatherName", ("Father's Name", s => s.FatherName) },
            { "MotherName", ("Mother's Name", s => s.MotherName) },
            { "AdmissionDate", ("Admission Date", s => s.AdmissionDate.ToString("dd-MM-yyyy")) },
            { "Email", ("Email", s => s.Email) },
            { "Gender", ("Gender", s => s.Gender?.Name) },
            { "PhoneNo", ("Phone", s => s.PhoneNo) },
            { "GuardianPhone", ("Guardian Phone", s => s.GuardianPhone) },
            { "DOB", ("Date of Birth", s => s.DOB.ToString("dd-MM-yyyy")) },
            { "Religion", ("Religion", s => s.Religion?.Name) },
            { "BloodGroup", ("Blood Group", s => s.BloodGroup?.Name) },
            { "PresentAddress", ("Present Address", s => $"{s.PresentAddressArea}, {s.PresentAddressPO}, {s.PresentUpazila?.Name}, {s.PresentDistrict?.Name}, {s.PresentDivision?.Name}") },
            { "PermanentAddress", ("Permanent Address", s => $"{s.PermanentAddressArea}, {s.PermanentAddressPO}, {s.PermanentUpazila?.Name}, {s.PermanentDistrict?.Name}, {s.PermanentDivision?.Name}") },
            { "AcademicSession", ("Session", s => s.AcademicSession?.Name) },
            { "PreviousSchool", ("Previous School", s => s.PreviousSchool) },
            { "IsResidential", ("Residential Type", s => s.IsResidential ? "Residential" : "Non-Residential") },
            { "SMSService", ("SMS Service", s => s.SMSService ? "Yes" : "No SMS") },
            { "Status", ("Status", s => s.Status ? "Active" : "Inactive") }
        };

        var reportColumns = new List<string> { "ClassRoll", "Name", "Column3", "Column4", "Column5", "Column6", "Column7", "Column8", "Column9", "AcademicClass", "AcademicSession", "ColumnExtra3", "ColumnExtra4", "ColumnExtra5" };

        // Create a mapping of fixed RDLC column names to selected dynamic columns
        var dynamicColumnMap = new Dictionary<string, string>();

        int dynamicStart = 2;
        for (int i = dynamicStart; i < 9; i++)
        {
            string fixedColumn = "Column" + (i + 1);
            if (columns.Count > i)
            {
                dynamicColumnMap[fixedColumn] = columns[i];
            }
            else
            {
                dynamicColumnMap[fixedColumn] = fixedColumn;
            }
        }
        var mandatoryColumns = new List<string> { "Name", "ClassRoll", "AcademicClass", "AcademicSession" };
        foreach (var col in mandatoryColumns)
            if (!columns.Contains(col)) columns.Insert(0, col);

        var dataTable = new DataTable("StudentDynamicReportDataset");
        reportColumns.ForEach(c => dataTable.Columns.Add(c));

        foreach (var student in students.OrderBy(s => s.AcademicClassId).ThenBy(s => s.ClassRoll))
        {
            var row = dataTable.NewRow();


            // Mandatory Fields
            row["ClassRoll"] = student.ClassRoll;
            row["Name"] = student.Name;
            row["AcademicClass"] = student.AcademicClass?.Name ?? "";
            row["AcademicSession"] = student.AcademicSession?.Name ?? "";

            // Dynamic Mapped Columns
            foreach (var map in dynamicColumnMap)
            {
                var fixedCol = map.Key;
                var originalCol = map.Value;

                if (columnMappings.TryGetValue(originalCol, out var colInfo))
                {
                    if (fixedCol == "Name" || fixedCol == "ClassRoll")
                    {
                        continue;
                    }
                    else
                    {
                        row[fixedCol] = colInfo.Selector(student) ?? string.Empty;
                    }
                }
            }

            dataTable.Rows.Add(row);
        }

        var reportPath = Path.Combine(_host.WebRootPath, "Reports", "RptStudentDynamicReport.rdlc");
        var imagePath = Path.Combine(_host.WebRootPath, "Images", "Institute", institute.Logo);
        string imageParam = string.Empty;

        if (System.IO.File.Exists(imagePath))
        {
            using var image = Image.FromFile(imagePath);
            using var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            imageParam = Convert.ToBase64String(ms.ToArray());
        }

        using var report = new LocalReport { ReportPath = reportPath };
        report.DataSources.Add(new ReportDataSource("StudentDynamicReportDataset", dataTable));

        var dynamicHeaderParams = dynamicColumnMap
        .Select((map, index) => new ReportParameter(
            $"Column{index + 3}Header",
            columnMappings.TryGetValue(map.Value, out var val) ? val.Label : map.Value
        ))
        .ToArray();

        var parameters = new[]
        {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("ReportName", "Student List"),
            new ReportParameter("Address", institute.Address),
            new ReportParameter("EIIN", institute.EIIN),
            new ReportParameter("Logo", imageParam)
        }.Concat(dynamicHeaderParams).ToArray();

        report.SetParameters(parameters);
        var result = report.Render(reportType ?? "pdf");
        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                result = report.Render("excel");
            }
            if (reportType == "word")
            {
                result = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd");
            return File(result, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(result, "application/pdf");
    }


    #endregion Student List Report

    #region Attendance Reports
    [Authorize(Policy = "DailyAttendanceReportsPolicy")]
    public async Task<IActionResult> DailyAttendanceReport()
    {
        ViewData["AcademicClass"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
        return View();
    }

    [Authorize(Policy = "DailyAttendanceReportsPolicy")]
    public async Task<IActionResult> DailyAttendnaceReportExport(string reportType, string fromDate, string academicClassId, string academicSectionId, string attendanceType, string fileName, string attendanceFor)
    {

        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }

        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\Rpt_Daily_Attendance.rdlc";

        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;
        var reportName = "Students Daily Attendance Report";

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        attendanceFor = attendanceFor == "s" ? "student" : "employees";
        AcademicSession academicSession = await _academicSessionManager.GetCurrentAcademicSession();

        List<RptDailyAttendaceVM> studentDailyAttendance = await _reportManager.GetDailyAttendanceReport(fromDate, academicClassId, academicSectionId, attendanceType, academicSession.Id.ToString(), attendanceFor);
        string totalStudents = studentDailyAttendance.Count.ToString();
        if (attendanceFor == "employees")
        {
            path = _host.WebRootPath + "\\Reports\\Rpt_Daily_Attendance_Employee.rdlc";
            reportName = "Employees Daily Attendance Report";
        }
        using var report = new Microsoft.Reporting.NETCore.LocalReport();
        if (studentDailyAttendance.Count > 0)
        {
            var allActiveStudents = await _studentManager.GetCurrentStudentListAsync(null, null);
            foreach (var item in studentDailyAttendance)
            {
                var isResidential = allActiveStudents.FirstOrDefault(s => s.ClassRoll.ToString() == item.CardNo.Trim())?.IsResidential;
                if (isResidential == true)
                {
                    item.Name = item.Name + " " + "(R)";
                }
            }
        }
        report.DataSources.Add(new ReportDataSource("AttendanceReportDS", studentDailyAttendance));
        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("Location", institute.Address),
            new ReportParameter("EIINNo", institute.EIIN),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("ReportName", reportName),
            new ReportParameter("AttendanceDate", fromDate),
            new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy")),
            new ReportParameter("TotalStudent",totalStudents)
        };
        report.ReportPath = path;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(fileName))
        {
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, mediaType);

    }
    //Monthly Attendance Report
    [Authorize(Policy = "AttendanceReportsPolicy")]
    public async Task<IActionResult> AttendanceReport()
    {
        ViewBag.AcademicClasslist = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
        string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;
        List<SelectListItem> monthsList = new();
        for (int i = 1; i <= 12; i++)
        {
            SelectListItem item = new()
            {
                Text = monthNames[i - 1],
                Value = i.ToString()
            };
            monthsList.Add(item);
        }
        ViewBag.Monthlist = new SelectList(monthsList, "Value", "Text");

        MonthlyAttendanceFullClass monthlyAttendanceFullClass = new();
        return View(monthlyAttendanceFullClass);
    }

    //Monthly Attendance Report
    [HttpPost]
    [Authorize(Policy = "AttendanceReportsPolicy")]
    public async Task<IActionResult> AttendanceReport(int monthId, int classId)
    {
        ViewBag.AcademicClasslist = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", classId).ToList();

        string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;
        List<SelectListItem> monthsList = new();
        for (int i = 1; i <= 12; i++)
        {
            SelectListItem item = new()
            {
                Text = monthNames[i - 1],
                Value = i.ToString()
            };
            monthsList.Add(item);
        }
        ViewBag.Monthlist = new SelectList(monthsList, "Value", "Text", monthId);

        string StartDate = string.Empty;
        string EndDate = string.Empty;
        string monthName = string.Empty;
        string className = string.Empty;
        // Create a new DateTime object for the first day of the month
        DateTime firstDateOfMonth = new(DateTime.Now.Year, monthId, 1);
        ViewBag.StartDate = firstDateOfMonth.ToString("dd-MMM-yyyy");
        // Get the last day of the month by adding one month to the first day and subtracting one day
        DateTime lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);
        StartDate = firstDateOfMonth.ToString("yyyy-MM-dd");
        EndDate = lastDateOfMonth.ToString("yyyy-MM-dd");

        //Get Month name by month number 
        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
        monthName = dfi.MonthNames[monthId - 1];

        //Get Class Name by ClasssId
        AcademicClass academicClass = await _academicClassManager.GetByIdAsync(classId);
        className = academicClass.Name;


        MonthlyAttendanceFullClass monthlyAttendanceFullClass = new()
        {
            MonthName = monthName,
            ClassName = className,
            MothlyAttendanceFullClassDetailses = new List<MonthlyAttendanceFullClassDetails>()
        };

        int monthDays = ViewBag.daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, monthId);

        var attendanceList = await _attendanceMachineManager.GetAttendanceByDateRangeAsync(StartDate, EndDate);
        AcademicSession academicSession = await _academicSessionManager.GetCurrentAcademicSession();
        var studentList = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(academicSession.Id, classId);

        List<DateTime> monthlyHolidays = await _OffDayManager.GetMonthlyHolidaysAsync(firstDateOfMonth.ToString("MMyyyy"));
        ViewBag.monthlyHolidays = monthlyHolidays;
        foreach (Student student in studentList.Where(s => s.Status == true))
        {
            var myAttendances = attendanceList.Where(t => Convert.ToInt32(t.CardNo) == Convert.ToInt32(student.UniqueId.Trim())).ToList();

            IDictionary<int, bool> daysPresents = new Dictionary<int, bool>();
            MonthlyAttendanceFullClassDetails monthlyAttendanceFullClassDetails = new()
            {
                StudentName = student.Name,
                Roll = student.ClassRoll,
                isPresents = daysPresents
            };


            for (int i = 1; i <= monthDays; i++)
            {
                var currentDate = firstDateOfMonth.AddDays(i - 1).ToString("ddMMyyyy");
                var attended = myAttendances.Where(a => a.PunchDatetime.ToString("ddMMyyyy") == currentDate).Any();
                if (attended)
                {
                    daysPresents.Add(i, true);
                }
                else
                {
                    daysPresents.Add(i, false);
                }
                monthlyAttendanceFullClassDetails.isPresents = daysPresents;
            }
            int total = daysPresents.Where(m => m.Value == true).Count();
            monthlyAttendanceFullClassDetails.Total = total;
            monthlyAttendanceFullClassDetails.CountPercentage = (total * 100) / (monthDays - monthlyHolidays.Count);
            monthlyAttendanceFullClassDetails.Holidays = monthlyHolidays;
            monthlyAttendanceFullClass.MothlyAttendanceFullClassDetailses.Add(monthlyAttendanceFullClassDetails);
        }

        ViewBag.studentList = studentList;
        return View(monthlyAttendanceFullClass);
    }

    public IActionResult MonthlyAttendanceReport()
    {
        return View();
    }

    public async Task<IActionResult> MonthlyAttendanceReportExport()
    {
        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }

        var students = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(5, 1);
        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\Rpt_Monthly_Attendance_Report.rdlc";

        using var report = new LocalReport();
        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        int monthId = 1;

        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
        var monthName = dfi.MonthNames[monthId - 1];

        DateTime firstDateOfMonth = new(DateTime.Now.Year, monthId, 1);

        // Get the last day of the month by adding one month to the first day and subtracting one day
        DateTime lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);
        var StartDate = firstDateOfMonth.ToString("yyyy-MM-dd");
        var EndDate = lastDateOfMonth.ToString("yyyy-MM-dd");

        var attendanceList = await _attendanceMachineManager.GetAttendanceByDateRangeAsync(StartDate, EndDate);
        List<RptMonthlyAttendanceVM> monthlyAttendance = new List<RptMonthlyAttendanceVM>();
        foreach (var student in students)
        {
            RptMonthlyAttendanceVM monthlyAttendanceVM = new RptMonthlyAttendanceVM
            {
                ClassRoll = student.ClassRoll.ToString(),
                StudentName = student.Name,
                Day1 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "01") ? "P" : ".",
                Day2 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "02") ? "P" : ".",
                Day3 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "03") ? "P" : ".",
                Day4 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "04") ? "P" : ".",
                Day5 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "05") ? "P" : ".",
                Day6 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "06") ? "P" : ".",
                Day7 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "07") ? "P" : ".",
                Day8 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "08") ? "P" : ".",
                Day9 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "09") ? "P" : ".",
                Day10 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "10") ? "P" : ".",
                Day11 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "11") ? "P" : ".",
                Day12 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "12") ? "P" : ".",
                Day13 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "13") ? "P" : ".",
                Day14 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "14") ? "P" : ".",
                Day15 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "15") ? "P" : ".",
                Day16 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "16") ? "P" : ".",
                Day17 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "17") ? "P" : ".",
                Day18 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "18") ? "P" : ".",
                Day19 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "19") ? "P" : ".",
                Day20 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "20") ? "P" : ".",
                Day21 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "21") ? "P" : ".",
                Day22 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "22") ? "P" : ".",
                Day23 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "23") ? "P" : ".",
                Day24 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "24") ? "P" : ".",
                Day25 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "25") ? "P" : ".",
                Day26 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "26") ? "P" : ".",
                Day27 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "27") ? "P" : ".",
                Day28 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "28") ? "P" : ".",
                Day29 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "29") ? "P" : ".",
                Day30 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "30") ? "P" : ".",
                Day31 = attendanceList.Any(s => s.CardNo == student.UniqueId && s.PunchDatetime.ToString("dd") == "31") ? "P" : ".",

            };
            monthlyAttendance.Add(monthlyAttendanceVM);
        }
        report.DataSources.Add(new ReportDataSource("DataSet1", monthlyAttendance));
        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("Location", institute.Address),
            new ReportParameter("EIINNo", institute.EIIN),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("ReportName", "Monthly Attendance Report"),
            new ReportParameter("MonthName",monthName),
            new ReportParameter("ClassName", "Class 6(Six)"),
            //new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy")),
            new ReportParameter("TotalDays",lastDateOfMonth.ToString("dd"))
        };
        report.ReportPath = path;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        report.ReportPath = path;
        //if (!string.IsNullOrEmpty("fileName"))
        //{
        //    return File(pdf, MediaTypeNames.Application.Octet, GetReportName("fileName", reportType));
        //}
        return File(pdf, mediaType);
    }

    public IActionResult DailyCheckoutReport()
    {
        DailyCheckoutReportSearchVM report = new DailyCheckoutReportSearchVM();

        return View(report);
    }

    [HttpPost]
    public async Task<IActionResult> DailyCheckoutReportExport(DailyCheckoutReportSearchVM dailyCheckoutReportVM)
    {

        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }

        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\Rpt_Daily_CheckOut.rdlc";

        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;
        var reportName = "Students Daily Attendance Report";

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        using var report = new Microsoft.Reporting.NETCore.LocalReport();

        //report.DataSources.Add(new ReportDataSource("AttendanceReportDS", studentDailyAttendance));
        //var parameters = new[] {
        //    new ReportParameter("InstituteName", institute.Name),
        //    new ReportParameter("Location", institute.Address),
        //    new ReportParameter("EIINNo", institute.EIIN),
        //    new ReportParameter("Logo", imageParam),
        //    new ReportParameter("ReportName", reportName),
        //    new ReportParameter("AttendanceDate", dailyCheckoutReportVM.ReportDate.ToString("dd MMM yyyy")),
        //    new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy")),
        //    new ReportParameter("TotalStudent",totalStudents)
        //};
        report.ReportPath = path;
        //report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(dailyCheckoutReportVM.fileName))
        {
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(dailyCheckoutReportVM.fileName, dailyCheckoutReportVM.ReportType));
        }
        return File(pdf, mediaType);
    }
    #endregion Attendance Reports

    #region Result or MarkSheet
    [Authorize(Policy = "SubjectWiseMarkSheetReportsPolicy")]
    public async Task<IActionResult> SubjectWiseMarkSheet(string reportType, int examId, string fileName)
    {
        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();

        string mediaType = "application/pdf";
        var reportPath = _host.WebRootPath + "\\Reports\\Rpt_Subject_Wise_MarkSheet.rdlc";

        string imageParam = "";
        var instituteLogoPath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(instituteLogoPath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        var examDetails = await _reportManager.GetSubjectWiseMarkSheet(examId);
        if (examDetails == null)
        {
            return new JsonResult("Nothing Found");
        }
        using var report = new Microsoft.Reporting.NETCore.LocalReport();
        report.DataSources.Add(new ReportDataSource("DataSet1", examDetails.OrderBy(s => s.ClassRoll)));

        var examInfo = await _academicExamManager.GetByIdAsync(examId);
        if (examInfo == null)
        {
            return new JsonResult("Please provide proper information");
        }

        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("Location", institute.Address),
            new ReportParameter("EIINNo", institute.EIIN),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("ReportName", "Subject-wise Mark Sheet"),
            new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy")),
            new ReportParameter("AcademicClass", examInfo.AcademicClass.Name),
            new ReportParameter("ExamTeacher", examInfo.Employee.EmployeeName),
            new ReportParameter("SubjectName", examInfo.AcademicSubject.SubjectName),
            new ReportParameter("AcademicSession", examInfo.AcademicExamGroup.AcademicSession.Name),
            new ReportParameter("ExamGroupName", examInfo.AcademicExamGroup.ExamGroupName),
            new ReportParameter("TotalMarks", examInfo.TotalMarks.ToString()),
        };
        report.ReportPath = reportPath;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                pdf = report.Render("excel");
            }
            if (reportType == "word")
            {
                pdf = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("dd MMM yyyy");
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, mediaType);

    }

    [Authorize(Policy = "SubjectWiseMarkSheetReportsPolicy")]
    public async Task<IActionResult> StudentWiseMarkSheet(string reportType, int examGroupId, int classId, string fileName)
    {
        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();

        string mediaType = "application/pdf";
        var reportPath = _host.WebRootPath + "\\Reports\\Rpt_Student_Wise_MarkSheet.rdlc";

        string imageParam = "";
        var instituteLogoPath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(instituteLogoPath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        var examDetails = await _reportManager.GetStudentWiseMarkSheet(examGroupId, classId);
        if (examDetails == null)
        {
            return new JsonResult("Nothing Found");
        }
        using var report = new LocalReport();
        var gradingTable = await _gradingTableManager.GetAllAsync();
        AcademicExamGroup academicExamGroup = await _academicExamGroupManager.GetByIdAsync(examGroupId);

        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("Location", institute.Address),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("EIINNo", institute.EIIN),
            new ReportParameter("ExamGroupName", academicExamGroup.ExamGroupName),
            new ReportParameter("ReportName", "ACADEMIC TRANSCRIPT"),
            new ReportParameter("Signature", "signature"),
            new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy")),
        };
        try
        {
            report.ReportPath = reportPath;
            report.SetParameters(parameters);
            report.DataSources.Add(new ReportDataSource("DS_Results", examDetails));
            report.DataSources.Add(new ReportDataSource("GradingTable_DataSet", gradingTable));
        }
        catch (Exception)
        {
            throw;
        }
        var pdf = report.Render("pdf");

        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                pdf = report.Render("excel");
            }
            if (reportType == "word")
            {
                pdf = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("dd MMM yyyy");
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, mediaType);
    }

    [Authorize(Policy = "StudentWiseMarkSheetReportsPolicy")]
    public async Task<IActionResult> MarkSheetReport()
    {
        GlobalUI.PageTitle = "Auto Mark-Sheet Generate";
        ViewBag.SessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name").ToList();
        ViewBag.ClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
        ViewBag.ExamGroupList = new SelectList(await _academicExamGroupManager.GetAllAsync(), "Id", "ExamGroupName").ToList();
        return View();
    }

    [Authorize(Policy = "StudentWiseMarkSheetReportsPolicy")]
    public async Task<IActionResult> MarkSheetReportExport(string reportType, string fileName, int examGroupId, int academicClassId, int? sectionId, int sessionId, int studentId)
    {
        var results = await _reportManager.GetStudentWiseMarkSheet(examGroupId, academicClassId);
        if (results == null || results.Count <= 0)
        {
            return new JsonResult("Result not found");
        }

        if (sectionId != null || sectionId > 0)
        {
            results = results.Where(s => s.AcademicSectionId == sectionId).ToList();
        }
        if (studentId > 0)
        {
            results = results.Where(s => s.StudentId == studentId).ToList();
        }
        if (results == null || results.Count <= 0)
        {
            return new JsonResult("Result not found");
        }
        AcademicExamGroup academicExamGroup = await _academicExamGroupManager.GetByIdAsync(examGroupId);
        AcademicClass academicClass = await _academicClassManager.GetByIdAsync(academicClassId);

        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();

        string imageParam = "";
        var instituteLogoPath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(instituteLogoPath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        StringBuilder stringBuilderMediaType = new StringBuilder();
        stringBuilderMediaType.Append("application/pdf");

        RenderType renderType = RenderType.Pdf;
        renderType = !string.IsNullOrEmpty(reportType) ? GetRenderType(reportType) : renderType;
        var path = _host.WebRootPath + "\\Reports\\Rpt_MarkSheet.rdlc";

        using var report = new Microsoft.Reporting.NETCore.LocalReport();
        report.DataSources.Add(new ReportDataSource("DataSet1", results));

        string publicationDate = results.Select(r => r.CreatedAt).FirstOrDefault().ToString("dd MMM yyyy");
        try
        {
            var parameters = new[] {
                new ReportParameter("InstituteName", institute.Name),
                new ReportParameter("Address", institute.Address),
                new ReportParameter("InstituteLogo", imageParam),
                new ReportParameter("EIINNo", institute.EIIN),
                new ReportParameter("ExamName", academicExamGroup.ExamGroupName),
                new ReportParameter("ClassName", academicClass.Name),
                new ReportParameter("PublicationDate",  publicationDate)
            };
            report.ReportPath = path;
            report.SetParameters(parameters);

            //For Sub Report
            report.SubreportProcessing += new SubreportProcessingEventHandler(SubReportAnnualReportProcessingAsync);


            TempData["gTables"] = await _gradingTableManager.GetAllAsync();
            report.SubreportProcessing += new SubreportProcessingEventHandler(SubReportGraddingTableProcessingAsync);
        }
        catch (Exception ex)
        {
            return new JsonResult(ex.InnerException.Message);
            throw;
        }

        var pdf = report.Render("pdf");

        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                pdf = report.Render("excel");
            }
            if (reportType == "word")
            {
                pdf = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("dd MMM yyyy");
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, stringBuilderMediaType.ToString());
    }
    void SubReportGraddingTableProcessingAsync(object sender, SubreportProcessingEventArgs e)
    {
        var gTables = TempData["gTables"];
        ReportDataSource reportDataSource = new ReportDataSource("GradingTable_DataSet", gTables);
        e.DataSources.Add(reportDataSource);
    }
    void SubReportAnnualReportProcessingAsync(object sender, SubreportProcessingEventArgs e)
    {
        if (e.ReportPath == "rptAnnualReport")
        {
            var stId = int.Parse(e.Parameters["StudentId"].Values[0]);
            var data = GetAnnualReportData(stId).GetAwaiter().GetResult();
            ReportDataSource reportDataSource = new ReportDataSource("AnnualReportDS", data);
            e.DataSources.Add(reportDataSource);
        }
    }
    #endregion Result or MarkSheet

    #region Student Payment Reports
    [Authorize(Policy = "StudentPaymentInfoReportsPolicy")]
    public IActionResult StudentPaymentInfo()
    {
        return View();
    }

    [Authorize(Policy = "StudentPaymentInfoReportsPolicy")]
    public async Task<IActionResult> StudentPaymentInfoExport(string reportType, string fileName, int classRoll, string fromDate, string toDate)
    {
        Student student = await _studentManager.GetStudentByClassRollAsync(classRoll);
        if (student == null)
        {
            return new JsonResult("Sorry! Student Data Not Found");
        }
        DateTime fdate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime tdate = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var studentPayments = await _reportManager.GetStudentPaymentsByRoll(classRoll, fdate.ToString("yyyy-MM-dd"), tdate.ToString("yyyy-MM-dd"));
        if (studentPayments.Count == 0)
        {
            return new JsonResult("Sorry! Any Payment Data Not Found");
        }

        double totalPaid = studentPayments.Sum(s => s.TotalPayment);
        string numberToWord = NumberToWords.ConvertAmount(totalPaid);
        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }

        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\rptStudentPaymentFullInfo.rdlc";

        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }

        using var report = new LocalReport();
        report.DataSources.Add(new ReportDataSource("DataSet1", studentPayments));
        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("InstituteAddress", institute.Address),
            new ReportParameter("StudentName", student.Name),
            new ReportParameter("ClassName", student.AcademicClass.Name),
            new ReportParameter("Session", student.AcademicSession.Name),
            new ReportParameter("RptDate", DateTime.Today.ToString("dd MMM yyyy")),
            new ReportParameter("RptName", "Payments Summary Report"),
            new ReportParameter("ClassRoll", classRoll.ToString()),
            new ReportParameter("AmountInWord", numberToWord),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("EIINNo", institute.EIIN)
        };
        report.ReportPath = path;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(fileName))
        {
            if (reportType == "xls")
            {
                pdf = report.Render("excel");
            }
            if (reportType == "word")
            {
                pdf = report.Render("word");
            }
            fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd");
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
        }
        return File(pdf, mediaType);
    }
    [Authorize(Policy = "StudentPaymentReportsPolicy")]
    public async Task<IActionResult> StudentPaymentReport()
    {
        ViewData["AcademicClass"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();

        return View();
    }

    [Authorize(Policy = "StudentPaymentReportsPolicy")]
    public async Task<IActionResult> StudentPaymentReportExport(string reportType, string fileName, string fromDate, string toDate, string academicClassId, string academicSectionId, string paymentCategory, int? paymentType)
    {
        string reportName = "Payments Report";
        var sPayment = await _reportManager.GetStudentPayment(fromDate, toDate, academicClassId, academicSectionId);
        if (paymentType != null)
        {
            var feeHead = await _studentFeeHeadManager.GetByIdAsync((int)paymentType);
            if (feeHead != null)
            {
                sPayment = sPayment.Where(s => s.PaymentType == feeHead.Name).ToList();
            }
        }
        if (paymentCategory == "residential")
        {
            sPayment = sPayment.Where(s => s.IsResidential).ToList();
            reportName = "Payments Summary Report(Residential)";
        }
        else if (paymentCategory == "nonResidential")
        {
            sPayment = sPayment.Where(s => s.IsResidential == false).ToList();
            reportName = "Payments Summary Report(Non Residential)";
        }
        else
        {
            reportName = "Payments Summary Report(Residential and Non Residential)";
        }
        if (sPayment.Count == 0)
        {
            return new JsonResult("Sorry! Data Not Found");
        }
        double amount = sPayment.Sum(m => m.TotalPayment);
        Institute institute = await _instituteManager.GetByIdAsync(1);
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }
        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\" + institute.Logo;
        var reportPath = _host.WebRootPath + "\\Reports\\Rpt_Student_Payment.rdlc";
        byte[] pdf;
        string mediaType = "application/pdf";
        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }

        using var report = new LocalReport();
        report.Refresh();
        try
        {
            report.DataSources.Add(new ReportDataSource("dsStudentPayment", sPayment));
            var parameters = new[] {
                new ReportParameter("InstituteName", institute.Name),
                new ReportParameter("Location", institute.Address),
                new ReportParameter("ReportName", reportName),
                new ReportParameter("FromDate", fromDate),
                new ReportParameter("ToDate", toDate),
                new ReportParameter("EIINNo", institute.EIIN),
                new ReportParameter("Logo", imageParam),
                new ReportParameter("AmountInWord", NumberToWords.ConvertAmount(amount)),
            };

            report.ReportPath = reportPath;
            report.SetParameters(parameters);
            pdf = report.Render("pdf");
            if (!string.IsNullOrEmpty(fileName))
            {
                if (reportType == "xls")
                {
                    pdf = report.Render("excel");
                }
                if (reportType == "word")
                {
                    pdf = report.Render("word");
                }
                fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd");
                return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
        }
        catch (Exception)
        {
            throw;
        }
        return File(pdf, mediaType);

    }

    [Authorize(Policy = "ReceiptPaymentReportsPolicy")]
    public async Task<IActionResult> ReceiptPaymentExport(string reportType, int paymentId, string myFileName)
    {
        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        if (institute == null)
        {
            return new JsonResult("Institute Information not found!");
        }
        string mediaType = "application/pdf";
        var path = _host.WebRootPath + "\\Reports\\Rpt_Payment_Receipt.rdlc";

        string imageParam = "";
        var imagePath = _host.WebRootPath + "\\Images\\Institute\\institute.jpeg";

        Image image = Image.FromFile(imagePath);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, image.RawFormat);
            byte[] imageBytes = ms.ToArray();
            imageParam = Convert.ToBase64String(imageBytes);
        }
        List<RptPaymentReceiptVM> rptPaymentReceiptVMs = await _reportManager.GetPaymentReceiptReport(paymentId);

        string numberToWord = string.Empty;
        if (rptPaymentReceiptVMs != null && rptPaymentReceiptVMs.Count > 0)
        {
            numberToWord = NumberToWords.ConvertAmount(rptPaymentReceiptVMs.Select(s => s.TotalPayment).FirstOrDefault());
        }

        using var report = new LocalReport();
        report.DataSources.Add(new ReportDataSource("Payment_Receipt_DataSet", rptPaymentReceiptVMs));
        var parameters = new[] {
            new ReportParameter("InstituteName", institute.Name),
            new ReportParameter("InstituteAddress", institute.Address),
            //new ReportParameter("EIINNo", institute.EIIN),
            new ReportParameter("Logo", imageParam),
            new ReportParameter("ReportName", "Payment Receipt"),
            new ReportParameter("AmountInWord", numberToWord)
        };
        report.ReportPath = path;
        report.SetParameters(parameters);
        var pdf = report.Render("pdf");
        if (!string.IsNullOrEmpty(myFileName) && myFileName.Length > 0)
        {
            return File(pdf, MediaTypeNames.Application.Octet, GetReportName(myFileName, reportType));
        }
        return File(pdf, mediaType);
    }
    #endregion Student Payment Reports

    #region Admit Card Reports
    [Authorize(Policy = "AdmitCardReportsPolicy")]
    public async Task<IActionResult> AdmitCardExport(string reportType, string fileName, int monthId, string academicClassId, string academicSectionId)
    {
        // Step 1: Fetch Institute Info
        var institute = await _instituteManager.GetByIdAsync(1);
        if (institute == null)
        {
            return new JsonResult("Institute information not found!");
        }

        // Step 2: Prepare Image and Signature as Base64 strings
        string imageParam = ConvertImageToBase64(Path.Combine(_host.WebRootPath, "Images", "Institute", institute.Logo));
        string signatureParam = ConvertImageToBase64(Path.Combine(_host.WebRootPath, "Images", "Institute", "signature.jpg"));

        // Step 3: Determine Report Format (default is PDF)
        var renderType = string.IsNullOrEmpty(reportType) ? RenderType.Pdf : GetRenderType(reportType);

        // Step 4: Load the RDLC Report
        var reportPath = Path.Combine(_host.WebRootPath, "Reports", "Rpt_AdmitCard.rdlc");
        using var localReport = new Microsoft.Reporting.NETCore.LocalReport
        {
            ReportPath = reportPath
        };

        // Step 5: Prepare Parameters for the RDLC Report
        var parameters = new Dictionary<string, string>
        {
            { "InstituteName", institute.Name },
            { "EIINNo", institute.EIIN },
            { "Image", imageParam },
            { "signature", signatureParam }
        };
        localReport.SetParameters(parameters.Select(p => new ReportParameter(p.Key, p.Value)).ToList());

        // Step 6: Parse Class & Section IDs
        int.TryParse(academicClassId, out int aClassId);
        int.TryParse(academicSectionId, out int aSectionId);

        // Step 7: Fetch Admit Card Data
        var admitCardList = await _reportManager.GetAdmitCard(monthId, aClassId, aSectionId);
        admitCardList = admitCardList.Where(s => s.StudentStauts == true).ToList();

        if (!admitCardList.Any())
        {
            return new JsonResult("No data found");
        }

        // Step 8: Bind Dataset to Report
        localReport.DataSources.Add(new ReportDataSource("DSAdmitCard", admitCardList));

        // Step 9: Render the Report and Return as File
        var result = localReport.Render(reportType);
        var contentType = "application/pdf";

        return !string.IsNullOrEmpty(fileName)
            ? File(result, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType))
            : File(result, contentType);
    }

    #endregion Admit Card Reports

    #region Common Methods
    private static RenderType GetRenderType(string reportType)
    {
        var renderType = reportType.ToLower() switch
        {
            "word" => RenderType.Word,
            "xls" => RenderType.Excel,
            _ => RenderType.Pdf,
        };
        return renderType;
    }

    private static string GetReportName(string reportName, string reportType)
    {
        string outputFileName = reportType.ToUpper() switch
        {
            "XLS" => reportName + ".xls",
            "WORD" => reportName + ".doc",
            _ => reportName + ".pdf",
        };
        return outputFileName;
    }

    private async Task<List<SubRerportAnnualReport>> GetAnnualReportData(int studentId)
    {
        var student = await _studentManager.GetByIdAsync(studentId);
        var examGroups = await _academicExamGroupManager.GetBySession(student.AcademicSessionId);
        List<SubRerportAnnualReport> filteredData = new List<SubRerportAnnualReport>();
        if (examGroups != null)
        {
            foreach (var eGroup in examGroups)
            {
                var examResults = await _examResultManager.GetExamResultsByExamGroupNClassId(eGroup.Id, student.AcademicClassId);
                foreach (var rItem in examResults.Where(s => s.StudentId == student.Id))
                {
                    SubRerportAnnualReport subRerportAnnualReport = new SubRerportAnnualReport()
                    {
                        MonthSL = eGroup.ExamMonthId,
                        AttendancePercent = rItem.AttendancePercentage.ToString(),
                        MeritPosition = rItem.Rank,
                        TotalStudent = examResults.Count()
                    };
                    filteredData.Add(subRerportAnnualReport);
                }
            }
        }
        List<SubRerportAnnualReport> data = new List<SubRerportAnnualReport>();
        DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        int monthSL = 1;
        foreach (var monthName in dateTimeFormat.MonthNames)
        {
            var fData = filteredData.FirstOrDefault(s => s.MonthSL == monthSL);
            if (!string.IsNullOrEmpty(monthName))
            {
                data.Add(new SubRerportAnnualReport()
                {
                    Month = monthName,
                    MonthSL = monthSL,
                    //AttendancePercent = "",
                    //MeritPosition = 0,
                    //TotalStudent = 90
                    AttendancePercent = fData?.AttendancePercent,
                    MeritPosition = fData?.MeritPosition,
                    TotalStudent = fData?.TotalStudent
                });
                monthSL++;
            }
        }
        return data;
    }
    #endregion Common Methods

    #region Gradingtable
    public async Task<IActionResult> GradingTable()
    {
        var gTables = await _gradingTableManager.GetAllAsync();
        LocalReport localSubReport = new LocalReport();
        localSubReport.ReportPath = _host.WebRootPath + "//Reports/Rpt_GradingTable.rdlc";
        localSubReport.DataSources.Add(new ReportDataSource("GradingTable_DataSet", gTables));
        var pdf = localSubReport.Render("pdf");
        return File(pdf, "application/pdf");
    }
    #endregion GradingTable

    #region Annual Report
    public async Task<IActionResult> AnnualReport()
    {
        var data = await GetAnnualReportData(1920);
        LocalReport localSubReport = new LocalReport();

        var parameters = new[] {
            new ReportParameter("StudentId", 1920.ToString()),
        };
        localSubReport.ReportPath = _host.WebRootPath + "//Reports/rptAnnualReport.rdlc";
        localSubReport.SetParameters(parameters);


        localSubReport.DataSources.Add(new ReportDataSource("AnnualReportDS", data));
        var pdf = localSubReport.Render("pdf");
        return File(pdf, "application/pdf");
    }
    #endregion Annual Report

    private string ConvertImageToBase64(string imagePath)
    {
        using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath);
        using var ms = new MemoryStream();
        image.Save(ms, new PngEncoder());
        return Convert.ToBase64String(ms.ToArray());
    }
}
