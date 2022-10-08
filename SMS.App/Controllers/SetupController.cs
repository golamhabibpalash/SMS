using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class SetupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SMSControl()
        {
            return View();
        }
    }
}
