using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class PhoneSMSController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
