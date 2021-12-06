using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            HttpContext.Session.SetString("UserId","user.Id");
            StudentDashboardVM stDashboardVM = new StudentDashboardVM();
            IReadOnlyCollection<Student> students = await _studentManager.GetAllAsync();
            stDashboardVM.Students = (ICollection<Student>)students;
            ViewBag.totalStudent = students.Count;

            IReadOnlyCollection<Employee> employee = await _employeeManager.GetAllAsync();
            ViewBag.totalEmployee = employee.Count();
            ViewBag.totalTeacher = employee.Where(g => g.DesignationId==1).Count();


            return View(stDashboardVM);
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
