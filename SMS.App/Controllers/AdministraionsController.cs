using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AdministraionsController : Controller
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdministraionsController(IEmployeeManager employeeManager,UserManager<ApplicationUser> userManager)
        {
            _employeeManager = employeeManager;
            _userManager = userManager;
        }
        public IActionResult AdminRegistration()
        {
            return View();
        }

        public async Task<IActionResult> UserProfile()
        {
            ViewData["EmployeeList"] = new SelectList(await _employeeManager.GetAllAsync(),"Id","Name");
            //await _userManager.u
            return View();
        }

        //[HttpPost]
        //public IActionResult UserProfile(int empId)
        //{
        //    ViewData["EmployeeList"] = new SelectList(await _employeeManager.GetAllAsync(), "Id", "Name");
        //    return View();
        //}
    }
}
