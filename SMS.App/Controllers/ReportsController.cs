using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using Serilog;
using SMS.App.Utilities.Others;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.ReportVM;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Reports;
using SMS.Entities;
using SMS.Entities.RptModels.AttendanceVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using LocalReport = Microsoft.Reporting.NETCore.LocalReport;

namespace SMS.App.Controllers
{
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
        #endregion properties

        #region Constructor
        public ReportsController(IWebHostEnvironment host, IStudentManager studentManager, IReportManager reportManager, IAcademicClassManager academicClassManager, IAttendanceMachineManager attendanceMachineManager, IOffDayManager dayManager, IAcademicSessionManager academicSessionManager, IAcademicSectionManager academicSectionManager, IInstituteManager instituteManager)
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
        }
        #endregion Constructor

        #region Student List Report
        public async Task<IActionResult> StudentsReport()
        {
            Rpt_Student_VM rpt_Student_VM = new()
            {
                AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList(),
                AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name").ToList()
            };
            return View(rpt_Student_VM);
        }
                
        public async Task<IActionResult> StudentsReportExport(string reportType,string fileName, int? academicClassId, int? academicSectionId)
        {
            AcademicSession aSession = await _academicSessionManager.GetCurrentAcademicSession();
            if (aSession==null)
            {
                return new JsonResult("Current Session not set");
            }

            var studens = await _reportManager.GetStudentsInfo(aSession.Id, academicClassId,academicSectionId);
            if (studens==null || studens.Count<=0)
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
            var imagePath = _host.WebRootPath + "\\Images\\Institute\\institute.jpeg";

            Image image = Image.FromFile(imagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();
                imageParam = Convert.ToBase64String(imageBytes);
            }

            using var report = new LocalReport();
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
                return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(pdf, mediaType);
        }
        #endregion Student List Report

        #region Attendance Reports

        public async Task<IActionResult> DailyAttendanceReport()
        {
            ViewData["AcademicClass"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
            return View();
        }

        public async Task<IActionResult> DailyAttendnaceReportExport(string reportType, string fromDate, string academicClassId, string academicSectionId,string attendanceType)
        {
            string fileName = string.Empty;

            Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
            if (institute == null)
            {
                return new JsonResult("Institute Information not found!");
            }

            string mediaType = "application/pdf";
            var path = _host.WebRootPath + "\\Reports\\Rpt_Daily_Attendance.rdlc";

            string imageParam = "";
            var imagePath = _host.WebRootPath + "\\Images\\Institute\\institute.jpeg";

            Image image = Image.FromFile(imagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();
                imageParam = Convert.ToBase64String(imageBytes);
            }
            List<RptDailyAttendaceVM> studentDailyAttendance = await _reportManager.GetDailyAttendanceReport(fromDate,academicClassId,academicSectionId,attendanceType);
            using var report = new LocalReport();
            report.DataSources.Add(new ReportDataSource("DSAttendanceReport", studentDailyAttendance));
            var parameters = new[] {
                new ReportParameter("InstituteName", institute.Name),
                new ReportParameter("Location", institute.Address),
                new ReportParameter("EIINNo", institute.EIIN),
                new ReportParameter("Logo", imageParam),
                new ReportParameter("ReportName", "Payments Summary Report"),
                new ReportParameter("AttendanceDate", fromDate),
                new ReportParameter("ReportDate", DateTime.Today.ToString("dd MMM yyyy"))
            };
            report.ReportPath = path;
            report.SetParameters(parameters);
            var pdf = report.Render("pdf");
            if (!string.IsNullOrEmpty(fileName))
            {
                return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(pdf, mediaType);

            return View();
        }
        //Monthly Attendance Report
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
        public async Task<IActionResult> AttendanceReport(int monthId, int classId)
        {
            ViewBag.AcademicClasslist = new SelectList(await _academicClassManager.GetAllAsync(),"Id","Name",classId).ToList();

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
            ViewBag.Monthlist = new SelectList(monthsList,"Value","Text",monthId);

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
            monthName = dfi.MonthNames[monthId-1];

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
            foreach (Student student in studentList.Where(s => s.Status==true))
            {
                var myAttendances = attendanceList.Where(t => t.CardNo==student.ClassRoll.ToString().PadLeft(8,'0')).ToList();

                IDictionary<int, bool> daysPresents = new Dictionary<int, bool>();
                MonthlyAttendanceFullClassDetails monthlyAttendanceFullClassDetails = new()
                {
                    StudentName = student.Name,
                    Roll = student.ClassRoll,
                    isPresents = daysPresents
                };


                for (int i = 1; i <= monthDays; i++)
                {
                    var currentDate = firstDateOfMonth.AddDays(i-1).ToString("ddMMyyyy");
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
                monthlyAttendanceFullClassDetails.CountPercentage = (total*100) / (monthDays-monthlyHolidays.Count);
                monthlyAttendanceFullClassDetails.Holidays = monthlyHolidays;
                monthlyAttendanceFullClass.MothlyAttendanceFullClassDetailses.Add(monthlyAttendanceFullClassDetails);
            }
            
            ViewBag.studentList = studentList;
            return View(monthlyAttendanceFullClass);
        }
        #endregion Attendance Reports

        #region MarkSheet
        public IActionResult MarkSheetReport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MarkSheettReport(string reportType, string fileName)
        {
            return View();
        }
        public async Task<IActionResult> MarkSheettReportExport(string reportType, string fileName)
        {
            RenderType renderType = RenderType.Pdf;
            renderType = !string.IsNullOrEmpty(reportType) ? GetRenderType(reportType) : renderType;
            var path = _host.WebRootPath + "\\Reports\\rptMarkSheet.rdlc";
            Dictionary<string, string> parameters = new();
            //parameters.Add("rp1", "Welcome to RDLC Reporting");
            AspNetCore.Reporting.LocalReport localReport = new(path);
            //List<RptStudentVM> studentVMs = new List<RptStudentVM>();
            int AcdemicSessionId = 0;
            int? AcademicClassId = 0;
            int? AcademicSectionId = 0;
            var studens = await _reportManager.GetStudentsInfo(AcdemicSessionId, AcademicClassId,AcademicSectionId);
            localReport.AddDataSource("DataSet1", studens);
            var result = localReport.Execute(renderType, 1, parameters);
            if (!string.IsNullOrEmpty(fileName))
            {
                return File(result.MainStream, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(result.MainStream, "Application/pdf");
        }
        #endregion MarkSheet

        #region Student Payment Reports
        public async Task<IActionResult> StudentPaymentInfo()
        {
            return View();
        }
        public async Task<IActionResult> StudentPaymentInfoExport(string reportType,string fileName,int classRoll,string fromDate, string toDate)
        {
            Student student = await _studentManager.GetStudentByClassRollAsync(classRoll);
            if (student == null)
            {
                return new JsonResult("Sorry! Student Data Not Found");
            }
            var studentPayments = await _reportManager.GetStudentPaymentsByRoll(classRoll, fromDate, toDate);
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
            var imagePath = _host.WebRootPath + "\\Images\\Institute\\institute.jpeg";

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
            report.ReportPath =path;
            report.SetParameters(parameters);
            var pdf = report.Render("pdf");
            if (!string.IsNullOrEmpty(fileName))
            {
                return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(pdf, mediaType);
        }
        public async Task<IActionResult> StudentPaymentReport()
        {
            ViewData["AcademicClass"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
            return View();
        }
        public async Task<IActionResult> StudentPaymentReportExport(string reportType, string fileName, string fromDate, string toDate, string academicClassId, string academicSectionId)
        {

            var sPayment = await _reportManager.GetStudentPayment(fromDate,toDate, academicClassId,academicSectionId);
            if (sPayment.Count==0)
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
            var imagePath = _host.WebRootPath + "\\Images\\Institute\\institute.jpeg"; 
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
                    new ReportParameter("ReportName", "Payments Summary Report"),
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
                    return File(pdf, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
                }
            }
            catch (Exception)
            {
                throw;
            }            
            return File(pdf, mediaType);

        }
        #endregion Student Payment Reports

        #region Admit Card Reports
        public async Task<IActionResult> AdmitCardExport(string reportType, string fileName, int monthId, string academicClassId, string academicSectionId)
        {
            string imageParam = "";
            string signatureParam = "";
            var imagePath = _host.WebRootPath + "\\Images\\Institute\\admitLogo.jpg";
            var signaturePath = _host.WebRootPath + "\\Images\\Institute\\signature.jpg";
            using (var b = new Bitmap(imagePath))
            {
                using(var ms = new MemoryStream())
                {
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    imageParam = Convert.ToBase64String(ms.ToArray());
                }
            }
            using (var b = new Bitmap(signaturePath))
            {
                using(var ms = new MemoryStream())
                {
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    signatureParam = Convert.ToBase64String(ms.ToArray());
                }
            }
            RenderType renderType = RenderType.Pdf;
            renderType = !string.IsNullOrEmpty(reportType) ? GetRenderType(reportType) : renderType;
            var path = _host.WebRootPath + "\\Reports\\Rpt_AdmitCard.rdlc";
            Dictionary<string, string> parameters = new();
            AspNetCore.Reporting.LocalReport localReport = new(path);

            Institute institute = await _instituteManager.GetByIdAsync(1);
            if (institute == null)
            {
                return new JsonResult("Institute Information not found!");
            }
            parameters.Add("InstituteName", institute.Name);
            parameters.Add("EIINNo", institute.EIIN);
            parameters.Add("Image", imageParam);
            parameters.Add("signature", signatureParam);
            
            int aClassId = 0;
            int aSection = 0;
            
            int.TryParse(academicClassId, out aClassId);
            int.TryParse(academicSectionId, out aSection);
            

            var admitCard = await _reportManager.GetAdmitCard(monthId,aClassId,aSection);

            localReport.AddDataSource("DSAdmitCard", admitCard);
            var result = localReport.Execute(renderType, 1, parameters);
            if (!string.IsNullOrEmpty(fileName))
            {
                return File(result.MainStream, MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(result.MainStream, "Application/pdf");
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
        #endregion Common Methods
    }
}
