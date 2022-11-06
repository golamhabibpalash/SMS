using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class SetupController : Controller
    {
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        public SetupController(ISetupMobileSMSManager setupMobileSMSManager)
        {
            _setupMobileSMSManager = setupMobileSMSManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SMSControl()
        {
            SetupMobileSMS setupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(1);
            if (setupMobileSMS != null)
            {
                return View(setupMobileSMS);
            }
            return View();
        }
    }
}
