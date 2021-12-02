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
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AccountsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStudentManager _studentManager;
        private readonly IEmployeeManager _employeeManager;
        //private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentManager studentManager, IEmployeeManager employeeManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _roleManager = roleManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
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
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> UserLogin(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Trim().ToLower() == userId.Trim().ToLower());
            if (user != null)
            {
                if (user.EmailConfirmed == false)
                {
                    ViewBag.msg = "Your verification is incomplete yet!";
                }
            }
            else
            {
                ViewBag.msg = "You are not registered yet!";
            }
            return View();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.AppUser, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var userList = _userManager.Users;

                    var appUser = await userList.FirstOrDefaultAsync(u => u.UserName == model.AppUser);

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

        public IActionResult ResetPassword()
        {
            return View();
        }

        public IActionResult RoleList()
        {
            var roleList = _roleManager.Roles;
            return View(roleList);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName                    
                };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
            }
            return View();
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View();
        }

        [AllowAnonymous]
        //[HttpPost]
        public async Task<JsonResult> GetUserByUserType(char id)
        {
            if (id=='e')
            {
                var empList = await _employeeManager.GetAllAsync();
                var eList = from e in empList
                          select new {userType = 'e', name = e.EmployeeName, value = e.Id, image = e.Image, phone = e.Phone };
                return Json(eList);
            }
            else
            {
                var stuList = from s in await _studentManager.GetAllAsync()
                              select new { userType = 's', name = s.Name, value = s.Id, image = s.Photo, roll = s.ClassRoll };


                return Json(stuList);
            }
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
