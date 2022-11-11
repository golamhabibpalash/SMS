using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class AutomationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
