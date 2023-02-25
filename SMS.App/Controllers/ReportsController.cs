using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Reports;
using SMS.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IStudentManager _studentManager;
        private readonly IReportManager _reportManager;
        public ReportsController(IWebHostEnvironment host, IStudentManager studentManager, IReportManager reportManager)
        {
            _host = host;
            _studentManager = studentManager;
            _reportManager = reportManager;
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
    }
}
