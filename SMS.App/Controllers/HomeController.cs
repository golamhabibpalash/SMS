using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentManager _studentManager;

        public HomeController(ILogger<HomeController> logger, IStudentManager studentManager)
        {
            _logger = logger;
            _studentManager = studentManager;
        }

        public async Task<IActionResult> Index()
        {
            StudentDashboardVM stDashboardVM = new StudentDashboardVM();
            IReadOnlyCollection<Student> students = await _studentManager.GetAllAsync();
            ViewBag.totalStudent = students.Count;
            stDashboardVM.Students = (ICollection<Student>)students;

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
