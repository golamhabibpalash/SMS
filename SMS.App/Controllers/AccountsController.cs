using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SMS.App.ViewModels.AdministrationVM;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
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
        
        public IActionResult UserList()
        {
            var allUser = _userManager.Users;
            return View(allUser);
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

        [HttpPost][Authorize]
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
        public IActionResult ForgotPassword() 
        {
            return View();
        }

        [AllowAnonymous][HttpPost]        
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model) 
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                string name = "";
                if (user.UserType=='e')
                {
                    var emp = await _employeeManager.GetByIdAsync(user.ReferenceId);
                    name = emp.EmployeeName;
                    model.Name = name;
                }
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Accounts", new { email = model.Email, token = token }, Request.Scheme);
                    //Logger code
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Noble Residential School", "golamhabibpalash@hotmail.com"));
                    message.To.Add(new MailboxAddress(model.Name, model.Email));
                    message.Subject = "Reset Password";
                    message.Body = new TextPart("plain")
                    {
                        Text = $"Hey {model.Name}, To reset your password go to <a href={passwordResetLink}> here </a>"
                    };
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        public IActionResult OTPGenerate()
        {

            return View();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult OTPGenerate(OTPVM model)
        {
            return View();
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email ==null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user!=null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        public async Task<IActionResult> RoleList()
        {
            List<RoleListWIthUserVM> roleListWIthUserVMs = new List<RoleListWIthUserVM>();
            var allUser = _userManager.Users;
            var roleList = _roleManager.Roles;
            foreach (var role in roleList)
            {
                RoleListWIthUserVM roleListWIthUserVM = new RoleListWIthUserVM();
                roleListWIthUserVM.IdentityRole = role;
                var selectedUer =new List<ApplicationUser>();
                foreach (var user in allUser)
                {
                    if (await _userManager.IsInRoleAsync(user,role.Name))
                    {
                        selectedUer.Add(user);
                    }
                }
                roleListWIthUserVM.Users = selectedUer;
                roleListWIthUserVMs.Add(roleListWIthUserVM);
            }
            return View(roleListWIthUserVMs);
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

        [HttpGet]        
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Not Found");
            }
            EditRoleVM editRoleVM = new() {
                Id = role.Id,
                RoleName = role.Name,
                ApplicationUsers = new List<string>()
            };

            //Retrive all the users
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    editRoleVM.ApplicationUsers.Add(user.UserName);
                }
            }

            return View(editRoleVM);
        }
        
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found.";
                return View("Not Found");
            }
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("EditRole",new {id=model.Id });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUser(string id)
        {
            var existRole = await _roleManager.FindByIdAsync(id);
            if (existRole == null)
            {
                ViewBag.ErrorMessage = $"Role with Id ={id} cannot be found";
                return View("Not Found");
            }
            ViewBag.RoleId = id;
            ViewBag.RoleName = existRole.Name;
            var model = new List<UserRoleVM>();
            foreach (var user in _userManager.Users)
            {
                var userRoleVm = new UserRoleVM
                { 
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserType = user.UserType
                };
                if (await _userManager.IsInRoleAsync(user, existRole.Name))
                {
                    userRoleVm.IsSelected = true;
                }
                else
                {
                    userRoleVm.IsSelected = false;
                }
                model.Add(userRoleVm);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUser(List<UserRoleVM> model,string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id ={roleId} cannot be found";
                return View("Not Found");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i<(model.Count-1))
                    {
                        continue;
                    }
                }
            }
            return RedirectToAction("EditRole",new { id = roleId});
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public async Task<JsonResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            string phoneNumber = "";
            if (user.UserType == 'e')
            {
                var emp = await _employeeManager.GetByIdAsync(user.ReferenceId);
                phoneNumber = emp.Phone;
            }
            if (user.UserType == 's')
            {
                var std = await _studentManager.GetByIdAsync(user.ReferenceId);
                phoneNumber = std.PhoneNo;
            }
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResetLink = Url.Action("ResetPassword", "Accounts", new { email = email, token = token }, Request.Scheme);
                return Json(new {token = token, link = passwordResetLink, phone = phoneNumber });                
            }
            return Json("");
        }
    }
}
