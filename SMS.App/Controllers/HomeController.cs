using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin, Teacher")]
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //private readonly IStudentManager _studentManager;
        //private readonly IEmployeeManager _employeeManager;
        //private readonly IAcademicClassManager _academicClassManager;
        //private readonly IDesignationManager _designationManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInstituteManager _instituteManager;
        //private readonly IAttendanceManager _attendanceManager;
        //private readonly IAttendanceMachineManager _attendanceMachineManager;
        //private readonly IStudentPaymentManager _studentPaymentManager;

        //public HomeController(ILogger<HomeController> logger, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager, IInstituteManager instituteManager, IAcademicClassManager academicClassManager, IDesignationManager designationManager, IAttendanceManager attendanceManager, IAttendanceMachineManager attendanceMachineManager, IStudentPaymentManager studentPaymentManager)
        //{
        //    _logger = logger;
        //    _studentManager = studentManager;
        //    _employeeManager = employeeManager;
        //    _userManager = userManager;
        //    _instituteManager = instituteManager;
        //    _academicClassManager = academicClassManager;
        //    _designationManager = designationManager;
        //    _attendanceManager = attendanceManager;
        //    _attendanceMachineManager = attendanceMachineManager;
        //    _studentPaymentManager = studentPaymentManager;
        //}
        public HomeController(UserManager<ApplicationUser> userManager, IInstituteManager instituteManager)
        {
            _userManager = userManager;
            _instituteManager = instituteManager;
        }
        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("macAddress", MACService.GetMAC());
            var user = await _userManager.GetUserAsync(User);
            HttpContext.Session.SetString("UserId",user.Id);
            Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
            ViewBag.InstituteLogo = institute.Logo;
            ViewBag.InstituteName = institute.Name;
            return View();
        }
        //public async Task<IActionResult> Index_Old()
        //{
        //    HttpContext.Session.SetString("macAddress", MACService.GetMAC());

        //    Institute institute = new Institute();
        //    var allInfo = await _instituteManager.GetAllAsync();
        //    if (allInfo.Count()>0)
        //    {
        //        institute = allInfo.FirstOrDefault();
        //        ViewBag.InstituteName = institute.Name;
        //    }
        //    var user = await _userManager.GetUserAsync(User);
        //    HttpContext.Session.SetString("UserId",user.Id);

        //    DashboardIndexVM DashboardVM = new DashboardIndexVM();
        //    IReadOnlyCollection<Student> students = await _studentManager.GetAllAsync();
        //    students = students.Where(s => s.Status == true).ToList();

        //    DashboardVM.Students = (ICollection<Student>)students;
        //    DashboardVM.Employees = (ICollection<Employee>)await _employeeManager.GetAllAsync();

        //    DashboardVM.Employees = DashboardVM.Employees.Where(s => s.Status== true).ToList(); 

        //    var todaysAllAttendance = await _attendanceMachineManager.GetAllAttendanceByDateAsync(DateTime.Now.Date);
        //    var todaysAllUniqeAttendance = todaysAllAttendance.GroupBy(a => a.CardNo).ToList();
        //    var allDesignations = await _designationManager.GetAllAsync();
        //    allDesignations = allDesignations.Where(d => d.Employees.Count() > 0).ToList();

        //    List<TodaysAttendanceEmpVM> todaysAttendanceEmpVMs = new List<TodaysAttendanceEmpVM>();
        //    foreach (var designation in allDesignations)
        //    {
        //        TodaysAttendanceEmpVM todaysAttendanceEmpVM = new TodaysAttendanceEmpVM();
        //        todaysAttendanceEmpVM.Designation = designation;
        //        todaysAttendanceEmpVM.AttendedEmployees = (from e in designation.Employees
        //                                                  from a in todaysAllUniqeAttendance
        //                                                  where a.Key == e.Phone.Substring(e.Phone.Length - 9) 
        //                                                  && e.Status == true
        //                                                  select e).ToList();
        //        todaysAttendanceEmpVM.TotalEmployee = designation.Employees.Where(e => e.Status==true).Count();
        //        todaysAttendanceEmpVMs.Add(todaysAttendanceEmpVM);
        //    }
        //    DashboardVM.TodaysAttendanceEmpVMs = todaysAttendanceEmpVMs;
        //    var allAcademicClass = await _academicClassManager.GetAllAsync();
            
        //    List<TodaysAttendanceStuVM> todaysAttendanceStuVMs = new List<TodaysAttendanceStuVM>();
        //    foreach (var aClass in allAcademicClass.Where(c => c.Students.Count() > 0))
        //    {
        //        TodaysAttendanceStuVM todaysAttendanceStuVM = new TodaysAttendanceStuVM();
        //        todaysAttendanceStuVM.AcademicClass = aClass;
        //        todaysAttendanceStuVM.AttendedStudents = (from s in aClass.Students
        //                                                  from a in todaysAllUniqeAttendance
        //                                                  where a.Key == s.ClassRoll.ToString().PadLeft(8, '0')
        //                                                  && s.Status == true
        //                                                  select s).ToList();
        //        todaysAttendanceStuVM.TotalStudent = aClass.Students.Count();

        //        todaysAttendanceStuVMs.Add(todaysAttendanceStuVM);
        //    }
        //    DashboardVM.TodaysAttendanceStuVMs = todaysAttendanceStuVMs;

        //    //Daily Collection Start here=====================
        //    List<PaymentCollection> dPaymentCollections = new List<PaymentCollection>();
        //    try
        //    {
        //        List<StudentPaymentSummeryVM> dailyPaymentSummery  = (List<StudentPaymentSummeryVM>) await _studentPaymentManager.GetPaymentSummeryByDate(DateTime.Now.ToString("ddMMyyyy"));
        //        foreach (var item in dailyPaymentSummery)
        //        {
        //            PaymentCollection dPC = new PaymentCollection();
        //            dPC.AcademicClassName = item.AcademicClassName;
        //            dPC.Amount = item.Payments;

        //            dPaymentCollections.Add(dPC);
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    DashboardVM.DailyCollections = dPaymentCollections;


        //    //Monthly Collection Start here ===================
        //    List<PaymentCollection> mPaymentCollections = new List<PaymentCollection>();
        //    try
        //    {
        //        List<StudentPaymentSummeryVM> monthlyPaymentSummery = (List<StudentPaymentSummeryVM>)await _studentPaymentManager.GetPaymentSummeryByMonthYear(DateTime.Now.ToString("MMyyyy"));
        //        foreach (var item in monthlyPaymentSummery)
        //        {
        //            PaymentCollection dPC = new PaymentCollection();
        //            dPC.AcademicClassName = item.AcademicClassName;
        //            dPC.Amount = item.Payments;
        //            mPaymentCollections.Add(dPC);
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    DashboardVM.MonthlyCollections = mPaymentCollections;
        //    return View(DashboardVM);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
