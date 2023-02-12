using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.WebForms;
using SMS.Entities;
using System.Collections.Generic;
using System.Data;

namespace SMS.App.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _host;
        public ReportsController(IWebHostEnvironment host)
        {
            _host = host;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentsReport()
        {
            var dt = new DataTable();
            dt = GetStudentList();
            var reportName = "rptStudent.rdlc";
            var path = _host.WebRootPath + "/Reports/"+ reportName;
            LocalReport localReport = new LocalReport();
            localReport.ReportPath= path;
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Value= dt;
            localReport.DataSources.Add(reportDataSource);
            

            return View();
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
    }
}
