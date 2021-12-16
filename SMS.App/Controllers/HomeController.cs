using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            HttpContext.Session.SetString("UserId","user.Id");
            DashboardIndexVM DashboardVM = new DashboardIndexVM();
            IReadOnlyCollection<Student> students = await _studentManager.GetAllAsync();
            DashboardVM.Students = (ICollection<Student>)students;
            DashboardVM.Employees = (ICollection<Employee>)await _employeeManager.GetAllAsync();
            DashboardVM.Classes = (ICollection<AcademicClass>)await _academicClassManager.GetAllAsync();
            DashboardVM.Designations = (ICollection<Designation>)await _designationManager.GetAllAsync();

            var allAttendances = await _attendanceManager.GetAllAsync();
            DashboardVM.Attendances = (ICollection<Attendance>)allAttendances.Where(a => a.AttendanceDate.Date.ToString() == DateTime.Today.Date.ToString()).ToList();

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
