using Microsoft.AspNetCore.Mvc;
using SMS.Entities;
using System.Collections.Generic;

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

        [HttpGet]
        public IActionResult GetCurrentStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetCurrentStudent(List<int> academicClassIds)
        {
            return View();
        }

    }
}
