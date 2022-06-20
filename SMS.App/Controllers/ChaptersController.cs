using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class ChaptersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
