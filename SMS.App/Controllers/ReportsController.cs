using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StudentsReport()
        {
            return View();
        }
    }
}
