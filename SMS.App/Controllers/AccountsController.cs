using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.App.ViewModels.AdministrationVM;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IStudentManager _studentManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IStudentManager studentManager, IEmployeeManager employeeManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _context = context;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Register(char userType)
        {
            if (userType=='e')
            {
                var employees = await _employeeManager.GetAllAsync();
                ViewBag.userList = new SelectList(employees, "Id", "EmployeeName");
                ViewBag.userType = userType;
                ViewBag.user = "Employee";
            }
            else if(userType == 's')
            {
                var students = await _studentManager.GetAllAsync();
                ViewBag.userList = new SelectList(students, "Id", "Name");
                ViewBag.userType = userType;
                ViewBag.user = "Student";
            }
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                string userName = "";
                if (model.UserType == 's')
                {
                    var student = await _studentManager.GetByIdAsync(model.ReferenceId);
                    userName = student.ClassRoll.ToString();
                }
                else
                {
                    userName = model.Email;
                }
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    ReferenceId = model.ReferenceId,
                    UserType = model.UserType
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("login", "Accounts");
                    //return RedirectToAction("Confirmation", "Accounts", new { userId = user.Id});
                }
            }
            return View();
        }
        
        [HttpGet, AllowAnonymous]
        public IActionResult Login(char userType)
        {
            ViewBag.userType = userType;
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginVm model)
        {
            ViewBag.userType = model.UserType;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.AppUser, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.UserName == model.AppUser);
                    IdentityUser identityUser = await _userManager.GetUserAsync(User);
                    var us = await _userManager.GetUserAsync(HttpContext.User);


                    if (appUser.UserType == 's')
                    {
                        return RedirectToAction("profile", "students", new { id = appUser.ReferenceId });
                    }
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login","Accounts");
        }

        public IActionResult Confirmation(string userId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Confirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetUserById(int id, string type)
        {
            
            if (type=="e")
            {
                Employee employee = await _employeeManager.GetByIdAsync(id);
                return Json(new { email = employee.Email, phone = employee.Phone });
            }
            else if (type =="s")
            {
                Student student = await _studentManager.GetByIdAsync(id);
                return Json(new {email = student.Email, phone=student.PhoneNo});
            }
            return Json("");
        }
    }
}
