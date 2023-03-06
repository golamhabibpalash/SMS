using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.ViewModels.AttendanceVM;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Reports;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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
        public ReportsController(IWebHostEnvironment host, IStudentManager studentManager, IReportManager reportManager, IAcademicClassManager academicClassManager, IAttendanceMachineManager attendanceMachineManager)
        {
            _host = host;
            _studentManager = studentManager;
            _reportManager = reportManager;
            _academicClassManager = academicClassManager;
            _attendanceMachineManager = attendanceMachineManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentsReport()
        {
            //var dt = new DataTable();
            //dt = GetStudentList();
            //var reportName = "rptStudent.rdlc";
            //var path = _host.WebRootPath + "/Reports/"+ reportName;
            //LocalReport localReport = new LocalReport();
            //localReport.ReportPath= path;
            //ReportDataSource reportDataSource = new ReportDataSource();
            //reportDataSource.Value= dt;
            //localReport.DataSources.Add(reportDataSource);
            

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StudentsReportAsync(string fileType)
        {
            string mimtype = "";
            int extension = 1;
            var path = _host.WebRootPath + "\\Reports\\rptStudent.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("rp1", "Welcome to RDLC Reporting");
            LocalReport localReport = new LocalReport(path);
            //List<RptStudentVM> studentVMs = new List<RptStudentVM>();

            var studens = await _reportManager.getStudentsInfo();

            localReport.AddDataSource("DataSet1", studens);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }

        [HttpGet]
        public IActionResult GetCurrentStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetCurrentStudent(List<int> academicClassIds)
        {
            return View();
        }
        public DataTable GetStudentList()
        {
            var dt = new DataTable();
            dt.Columns.Add("ClassRoll");
            dt.Columns.Add("Name");
            dt.Columns.Add("Class");
            dt.Columns.Add("Father");

            DataRow row;
            for (int i = 0; i < 120; i++)
            {
                row = dt.NewRow();
                row["ClassRoll"] = i;
                row["Name"] = "Student "+i;
                row["Class"] = "Class Name";
                row["Father"] = "Father Name"+i;
                dt.Rows.Add(row);
            }
            return dt;
        }

        //public IActionResult ExportData()
        //{
        //    var byteRes = new byte[] { };
        //    string path = _host.ContentRootPath + "\\Reports\\rptStudent.rdlc";
        //    byteRes = IReport
        //}

        public async Task<IActionResult> AttendanceReport()
        {
            ViewBag.AcademicClasslist = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;
            List<SelectListItem> monthsList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Text = monthNames[i - 1],
                    Value = i.ToString()
                };
                monthsList.Add(item);
            }
            ViewBag.Monthlist = new SelectList(monthsList, "Value", "Text");

            MonthlyAttendanceFullClass monthlyAttendanceFullClass = new MonthlyAttendanceFullClass();
            return View(monthlyAttendanceFullClass);
        }

        [HttpPost]
        public async Task<IActionResult> AttendanceReport(int monthId, int classId)
        {
            ViewBag.AcademicClasslist = new SelectList(await _academicClassManager.GetAllAsync(),"Id","Name",classId).ToList();
            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;
            List<SelectListItem> monthsList = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                SelectListItem item = new SelectListItem
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
            DateTime firstDateOfMonth = new DateTime(DateTime.Now.Year, monthId, 1);
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


            MonthlyAttendanceFullClass monthlyAttendanceFullClass = new MonthlyAttendanceFullClass();
            monthlyAttendanceFullClass.MonthName = monthName;
            monthlyAttendanceFullClass.ClassName = className;
            monthlyAttendanceFullClass.MothlyAttendanceFullClassDetailses = new List<MonthlyAttendanceFullClassDetails>();

            int monthDays = ViewBag.daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, monthId);

            var attendanceList = await _attendanceMachineManager.GetAttendanceByDateRangeAsync(StartDate, EndDate);
            
            var studentList = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(3, classId);
            foreach (Student student in studentList.Where(s => s.Status==true))
            {
                var myAttendances = attendanceList.Where(t => t.CardNo==student.ClassRoll.ToString().PadLeft(8,'0')).ToList();

                MonthlyAttendanceFullClassDetails monthlyAttendanceFullClassDetails = new MonthlyAttendanceFullClassDetails();
                monthlyAttendanceFullClassDetails.StudentName = student.Name;
                monthlyAttendanceFullClassDetails.Roll = student.ClassRoll;
                IDictionary<int, bool> daysPresents = new Dictionary<int, bool>();
                monthlyAttendanceFullClassDetails.isPresents = daysPresents;

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
                monthlyAttendanceFullClassDetails.countPercentage = (total*100) / monthDays;
                monthlyAttendanceFullClass.MothlyAttendanceFullClassDetailses.Add(monthlyAttendanceFullClassDetails);
            }
            
            
            ViewBag.studentList = studentList;
            return View(monthlyAttendanceFullClass);
        }
    }
}
