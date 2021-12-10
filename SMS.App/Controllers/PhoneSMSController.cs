using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class PhoneSMSController : Controller
    {
        private readonly IPhoneSMSManager _phoneSMSManager;

        public PhoneSMSController(IPhoneSMSManager phoneSMSManager)
        {
            _phoneSMSManager = phoneSMSManager;
        }
        public async Task<IActionResult> Index()
        {
            var allSMS = await _phoneSMSManager.GetAllAsync();
            return View(allSMS);
        }
    }
}
