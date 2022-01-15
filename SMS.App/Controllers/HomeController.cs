using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
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
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentManager _studentManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IDesignationManager _designationManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInstituteManager _instituteManager;
        private readonly IAttendanceManager _attendanceManager;

        public HomeController(ILogger<HomeController> logger, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager, IInstituteManager instituteManager, IAcademicClassManager academicClassManager, IDesignationManager designationManager, IAttendanceManager attendanceManager)
        {
            _logger = logger;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _userManager = userManager;
            _instituteManager = instituteManager;
            _academicClassManager = academicClassManager;
            _designationManager = designationManager;
            _attendanceManager = attendanceManager;
        }

        public async Task<IActionResult> Index()
        {
            Institute institute = new Institute();
            var allInfo = await _instituteManager.GetAllAsync();
            if (allInfo.Count()>0)
            {
                institute = allInfo.FirstOrDefault();
                ViewBag.InstituteName = institute.Name;
            }
            var user = await _userManager.GetUserAsync(User);
            HttpContext.Session.SetString("UserId",user.Id);

            DashboardIndexVM DashboardVM = new DashboardIndexVM();
            IReadOnlyCollection<Student> students = await _studentManager.GetAllAsync();
            DashboardVM.Students = (ICollection<Student>)students;
            DashboardVM.Employees = (ICollection<Employee>)await _employeeManager.GetAllAsync();

            var todaysAllAttendance = await _attendanceManager.GetTodaysAllAttendanceAsync();
            var todaysAllUniqeAttendance = todaysAllAttendance.GroupBy(a => a.CardNo).ToList();
            var allDesignations = await _designationManager.GetAllAsync();
            allDesignations = allDesignations.Where(d => d.Employees.Count() > 0).ToList();

            List<TodaysAttendanceEmpVM> todaysAttendanceEmpVMs = new List<TodaysAttendanceEmpVM>();
            foreach (var designation in allDesignations)
            {
                TodaysAttendanceEmpVM todaysAttendanceEmpVM = new TodaysAttendanceEmpVM();
                todaysAttendanceEmpVM.Designation = designation;
                todaysAttendanceEmpVM.AttendedEmployees = (from e in designation.Employees
                                                          from a in todaysAllUniqeAttendance
                                                          where a.Key == e.Phone.Substring(e.Phone.Length - 9) 
                                                          select e).ToList();
                todaysAttendanceEmpVM.TotalEmployee = designation.Employees.Count();
                todaysAttendanceEmpVMs.Add(todaysAttendanceEmpVM);
            }
            DashboardVM.TodaysAttendanceEmpVMs = todaysAttendanceEmpVMs;
            var allAcademicClass = await _academicClassManager.GetAllAsync();
            
            List<TodaysAttendanceStuVM> todaysAttendanceStuVMs = new List<TodaysAttendanceStuVM>();
            foreach (var aClass in allAcademicClass.Where(c => c.Students.Count() > 0))
            {
                TodaysAttendanceStuVM todaysAttendanceStuVM = new TodaysAttendanceStuVM();
                todaysAttendanceStuVM.AcademicClass = aClass;
                todaysAttendanceStuVM.AttendedStudents = (from s in aClass.Students
                                                          from a in todaysAllUniqeAttendance
                                                          where s.ClassRoll.ToString() == a.Key
                                                          select s).ToList();
                todaysAttendanceStuVM.TotalStudent = aClass.Students.Count();

                todaysAttendanceStuVMs.Add(todaysAttendanceStuVM);
            }
            DashboardVM.TodaysAttendanceStuVMs = todaysAttendanceStuVMs;
            return View(DashboardVM);
        }

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
