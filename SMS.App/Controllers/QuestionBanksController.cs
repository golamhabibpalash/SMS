using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class QuestionBanksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
