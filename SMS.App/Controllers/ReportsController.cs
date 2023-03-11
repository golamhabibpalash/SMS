using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.ReportVM;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Reports;
using SMS.Entities;
using Syncfusion.Pdf.Lists;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IStudentManager _studentManager;
        private readonly IReportManager _reportManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly IOffDayManager _OffDayManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly IAcademicSectionManager _academicSectionManager;
        public ReportsController(IWebHostEnvironment host, IStudentManager studentManager, IReportManager reportManager, IAcademicClassManager academicClassManager, IAttendanceMachineManager attendanceMachineManager, IOffDayManager dayManager, IAcademicSessionManager academicSessionManager, IAcademicSectionManager academicSectionManager)
        {
            _host = host;
            _studentManager = studentManager;
            _reportManager = reportManager;
            _academicClassManager = academicClassManager;
            _attendanceMachineManager = attendanceMachineManager;
            _OffDayManager = dayManager;
            _academicSessionManager = academicSessionManager;
            _academicSectionManager = academicSectionManager;
        }
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
        [HttpPost]
        public async Task<IActionResult> StudentsReport(Rpt_Student_VM rpt_Student_VMObject)
        {
            Rpt_Student_VM rpt_Student_VM = new()
            {
                AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", rpt_Student_VMObject.AcademicClassId).ToList(),
                AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", rpt_Student_VMObject.AcademicSessionId).ToList(),
                AcademicSectionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", rpt_Student_VMObject.AcademicSessionId).ToList()
            };
            return View(rpt_Student_VM);
        }
        
        public async Task<IActionResult> StudentsReportExport(string reportType,string fileName)
        {
            RenderType renderType = RenderType.Pdf;
            renderType = !string.IsNullOrEmpty(reportType)? GetRenderType(reportType):renderType;
            var path = _host.WebRootPath + "\\Reports\\rptStudent.rdlc";
            Dictionary<string, string> parameters = new();
            //parameters.Add("rp1", "Welcome to RDLC Reporting");
            LocalReport localReport = new(path);
            //List<RptStudentVM> studentVMs = new List<RptStudentVM>();

            var studens = await _reportManager.getStudentsInfo();
            localReport.AddDataSource("DataSet1", studens);
            var result = localReport.Execute(renderType, 1, parameters);
            if (!string.IsNullOrEmpty(fileName))
            {
            return File(result.MainStream,MediaTypeNames.Application.Octet, GetReportName(fileName, reportType));
            }
            return File(result.MainStream, "Application/pdf");
        }
        #endregion Student List Report

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
            
            var studentList = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(3, classId);

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

    }
}
