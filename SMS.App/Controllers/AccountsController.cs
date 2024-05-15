﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.App.Utilities.EmailServices;
using SMS.App.Utilities.MACIPServices;
using SMS.App.Utilities.ShortMessageService;
using SMS.App.ViewModels.AdministrationVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AccountsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStudentManager _studentManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IPhoneSMSManager _phoneSMSManager;
        private readonly IInstituteManager _instituteManager;
        //private readonly ApplicationDbContext _context;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentManager studentManager, IEmployeeManager employeeManager, RoleManager<IdentityRole> roleManager, IPhoneSMSManager phoneSMSManager, IInstituteManager instituteManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _roleManager = roleManager;
            _phoneSMSManager = phoneSMSManager;
            _instituteManager = instituteManager;
        }

        [HttpGet]
        [Authorize(Policy = "RegisterAccountsPolicy")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]

        [Authorize(Policy = "RegisterAccountsPolicy")]
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
                    EmailConfirmed = true,
                    PhoneNumber = model.Phone,
                    ReferenceId = model.ReferenceId,
                    UserType = model.UserType
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                string roleName = model.UserType == 's' ? "Student" : model.UserType == 'e' ? "Teacher" : "Other";
                result = await _userManager.AddToRoleAsync(user, roleName);

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
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
            }
            var allUser = _userManager.Users;
            return View(allUser);
        }

        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Policy = "EditUserAccountsPolicy")]
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            EditUserVM editUserVM = new EditUserVM()
            {
                Id = user.Id,
                ReferenceId = user.ReferenceId,
                UserName = user.UserName,
                NormalizedEmail = user.NormalizedEmail,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                UserType = user.UserType
            };
            return View(editUserVM);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Policy = "EditUserAccountsPolicy")]
        public async Task<IActionResult> EditUser(string id, EditUserVM model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            user.UserName = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            user.ReferenceId = model.ReferenceId;
            user.UserType = model.UserType;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Userlist");
            }
            return View(user);
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
        public IActionResult Login(string ReturnUrl = null)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            LoginVm model = new();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginVm model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Checking the user is entering user/mobile/email/roll
                    //01. If student enter with the currnet roll number
                    var result = await _signInManager.PasswordSignInAsync(model.AppUser, model.Password, model.RememberMe, false);
                    if (!result.Succeeded)
                    {
                        if (model.AppUser.Length == 7)
                        {
                            //search student with the roll number
                            Student existingStudent = await _studentManager.GetStudentByClassRollAsync(Convert.ToInt32(model.AppUser));
                            if (existingStudent != null)
                            {
                                result = await _signInManager.PasswordSignInAsync(existingStudent.UniqueId, model.Password, model.RememberMe, false);
                            }
                        }
                    }
                    if (result.Succeeded)
                    {
                        var userList = _userManager.Users;

                        var appUser = await userList.FirstOrDefaultAsync(u => u.UserName == model.AppUser);

                        if (appUser.UserType == 's')
                        {
                            return RedirectToAction("profile", "students", new { id = appUser.ReferenceId });
                        }
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                        }
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("exception", e.Message);
                    model.Error = e.Message;
                }

            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }


        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Policy = "DeleteUserAccountsPolicy")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            TempData["msg"] = "User Not Found";
            return RedirectToAction("UserList");
        }

        [HttpPost]
        [ActionName("DeleteUser")]
        [Authorize(Roles = "SuperAdmin")]

        [Authorize(Policy = "DeleteUserAccountsPolicy")]
        public async Task<IActionResult> ConfirmDelete(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["msg"] = "User is Deleted";
                    return RedirectToAction("UserList");
                }
                return View(user);
            }
            TempData["msg"] = "User Not Found";
            return RedirectToAction("UserList");
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewBag.msg = "please insert a varified email.";
                    return View(model);
                }
                string name = "";
                if (user.UserType == 'e')
                {
                    var emp = await _employeeManager.GetByIdAsync(user.ReferenceId);
                    name = emp.EmployeeName;
                    model.Name = name;
                }
                else if (user.UserType == 's')
                {
                    var student = await _studentManager.GetByIdAsync(user.ReferenceId);
                    name = student.Name;
                    model.Name = name;
                }
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Accounts", new { email = model.Email, token = token }, Request.Scheme);

                    HttpContext.Session.SetString("token", token);
                    HttpContext.Session.SetString("passwordResetLink", passwordResetLink);
                    HttpContext.Session.SetString("useremail", model.Email);

                    Random rnd = new Random();
                    int randomNumber = rnd.Next(100000, 999999);
                    HttpContext.Session.SetString("randomNumber", randomNumber.ToString());
                    var instituteInfo = await _instituteManager.GetAllAsync();
                    string text = "Your OTP is:" + randomNumber + " -" + instituteInfo.FirstOrDefault().Name;

                    if (model.verificationBy == "SMS")
                    {
                        bool smsSend = await MobileSMS.SendSMS(user.PhoneNumber, text);
                        if (smsSend == false)
                        {
                            ViewBag.msg = "SMS not sent due to technical problem, Try again.";
                            return View(model);
                        }
                        else if (smsSend == true)
                        {
                            PhoneSMS phoneSMS = new()
                            {
                                Text = text,
                                CreatedAt = DateTime.Now,
                                CreatedBy = model.Email,
                                MobileNumber = user.PhoneNumber,
                                MACAddress = MACService.GetMAC(),
                                SMSType = "OTP"
                            };
                            try
                            {
                                await _phoneSMSManager.AddAsync(phoneSMS);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }

                        return RedirectToAction("OTPGenerate");
                    }
                    else if (model.verificationBy == "Email")
                    {
                        bool isSend = EmailService.SendEmail(model.Email, "OTP for Password reset", text);
                        if (isSend)
                        {
                            return RedirectToAction("OTPGenerate");
                        }
                    }

                    ViewBag.link = passwordResetLink;
                }
                return View();
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult OTPGenerate()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public IActionResult OTPGenerate(OTPVM model)
        {
            var email = HttpContext.Session.GetString("useremail");
            model.Email = email;
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("token");

                if (model.OTP.ToString() == HttpContext.Session.GetString("randomNumber"))
                {
                    ResetPasswordVM resetPasswordVM = new ResetPasswordVM();
                    resetPasswordVM.Email = model.Email;
                    resetPasswordVM.Token = token;
                    HttpContext.Session.Remove("randomNumber");
                    HttpContext.Session.Remove("token");
                    HttpContext.Session.Remove("useremail");
                    return View("ResetPassword", resetPasswordVM);
                }
                else
                {
                    ViewBag.msg = "OTP is not matched";
                    return View(model);
                }
            }
            return RedirectToAction("ForgotPassword");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    ViewBag.msg = "Password not changed. Please input proper password";
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    TempData["msg"] = "User doesn't found";
                    return RedirectToAction("ForgotPassword");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        [Authorize(Policy = "ViewRolesAccountsPolicy")]
        public async Task<IActionResult> RoleList()
        {
            List<RoleListWIthUserVM> roleListWIthUserVMs = new List<RoleListWIthUserVM>();
            var allUser = _userManager.Users.Where(s => s.UserType == 'e');
            var roleList = _roleManager.Roles;
            foreach (var role in roleList)
            {
                RoleListWIthUserVM roleListWIthUserVM = new RoleListWIthUserVM();
                roleListWIthUserVM.IdentityRole = role;
                var selectedUer = new List<ApplicationUser>();
                foreach (var user in allUser)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
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
        [Authorize(Policy = "CreateRoleAccountsPolicy")]
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
        [Authorize(Policy = "EditRoleAccountsPolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Not Found");
            }
            EditRoleVM editRoleVM = new()
            {
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

        [Authorize(Policy = "EditRoleAccountsPolicy")]
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
                return RedirectToAction("EditRole", new { id = model.Id });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AddOrRemoveUserAccountsPolicy")]
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
            foreach (var user in _userManager.Users.Where(u => u.UserType == 'e'))
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
        [Authorize(Policy = "AddOrRemoveUserAccountsPolicy")]
        public async Task<IActionResult> AddOrRemoveUser(List<UserRoleVM> model, string roleId)
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
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                }
            }
            return RedirectToAction("EditRole", new { id = roleId });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetUserByUserType(char id)
        {
            if (id == 'e')
            {
                var empList = await _employeeManager.GetAllAsync();
                var eList = from e in empList
                            select new { userType = 'e', name = e.EmployeeName, value = e.Id, image = e.Image, phone = e.Phone };
                return Json(eList);
            }
            else
            {
                var stuList = from s in await _studentManager.GetAllAsync()
                              select new { userType = 's', name = s.Name, value = s.Id, image = s.Photo, roll = s.ClassRoll, phone = s.PhoneNo };


                return Json(stuList);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetUserById(int id, string type)
        {

            if (type == "e")
            {
                Employee employee = await _employeeManager.GetByIdAsync(id);
                return Json(new { email = employee.Email, phone = employee.Phone });
            }
            else if (type == "s")
            {
                Student student = await _studentManager.GetByIdAsync(id);
                return Json(new { email = student.Email, phone = student.PhoneNo });
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
                return Json(new { token = token, link = passwordResetLink, phone = phoneNumber });
            }
            return Json("");
        }
    }
}
