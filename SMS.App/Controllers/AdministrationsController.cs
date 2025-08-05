//using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem;
using SMS.App.ViewModels.AdministrationVM;
using SMS.App.ViewModels.ClaimContext;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AdministrationsController : Controller
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClaimStoreManager _claimStoreManager;
        private readonly IProjectModuleManager _projectModuleManager;
        public AdministrationsController(IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IClaimStoreManager claimStoreManager, IProjectModuleManager projectModuleManager)
        {
            _employeeManager = employeeManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimStoreManager = claimStoreManager;
            _projectModuleManager = projectModuleManager;
        }

        [Authorize(Policy = "ViewUserProfileAdministrationsPolicy")]
        public async Task<IActionResult> UserProfile(string userId)
        {
            GlobalUI.PageTitle = "User Profile";
            UserProfileVM userProfileVM = new UserProfileVM();
            var allUser = _userManager.Users.Where(s => s.UserType == 'e').OrderBy(s => s.UserName);
            ViewBag.UserList = userProfileVM.UserList = new SelectList(allUser, "Id", "UserName");

            UserRoleClaimsVM userRoleClaimsVM = new UserRoleClaimsVM();
            List<ProjectModule> modules = (List<ProjectModule>)await _projectModuleManager.GetAllAsync();
            if (userId != null)
            {
                ApplicationUser aUser = await _userManager.FindByIdAsync(userId);
                if (aUser != null)
                {
                    ViewBag.UserName = aUser.UserName;
                    var userIsInRole = await _userManager.GetRolesAsync(aUser);
                    var userInClaims = await _userManager.GetClaimsAsync(aUser);

                    Employee employee = await _employeeManager.GetByIdAsync(aUser.ReferenceId);
                    userProfileVM.Employee = employee;
                    //userRoleClaimsVM.ApplicationRoles = new List<SelectListItem>();
                    userRoleClaimsVM.ApplicationRoles = await _roleManager.Roles.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id,
                        Selected = userIsInRole.Contains(x.Id)
                    }).ToListAsync();

                    userRoleClaimsVM.ApplicationUser = aUser;
                    var claimStore = await _claimStoreManager.GetAllAsync();
                    userRoleClaimsVM.ApplicationClaims = claimStore.Select(s => new SelectListItem
                    {
                        Text = s.ClaimType,
                        Value = s.ClaimValue,
                        Selected = userInClaims.Any(c => c.Value == s.ClaimValue)
                    }).ToList();

                    userProfileVM.UserRoleClaimsVM = userRoleClaimsVM;
                    userProfileVM.ClaimStore = (List<ClaimStores>)claimStore;
                }
            }
            userProfileVM.Modules = modules;

            ViewData["EmployeeList"] = new SelectList(await _employeeManager.GetAllAsync(), "Id", "Name");
            return View(userProfileVM);
        }
        
        [HttpPost]
        [Authorize(Policy = "EditUserProfileAdministrationsPolicy")]
        //public async Task<IActionResult> UserProfile(UserProfileVM model)
        //{
        //    GlobalUI.PageTitle = "User Profile";
        //    var allUser = _userManager.Users.Where(s => s.UserType == 'e');

        //    ApplicationUser aUser = await _userManager.FindByIdAsync(model.UserRoleClaimsVM.ApplicationUser.Id);
        //    ViewBag.UserList = model.UserList = new SelectList(allUser, "Id", "UserName",aUser.Id);

        //    List<ProjectModule> modules = (List<ProjectModule>)await _projectModuleManager.GetAllAsync();
        //    if (aUser != null)
        //    {
        //        try
        //        {
        //            var claimStore = await _claimStoreManager.GetAllAsync();

        //            List<Claim> allClaim = new List<Claim>();
        //            foreach (var item in claimStore)
        //            {
        //                allClaim.Add(new Claim(item.ClaimType, item.ClaimValue));
        //            }
        //            List<Claim> selectedClaim = new List<Claim>();

        //            var selectedClaimValues = model.UserRoleClaimsVM.ApplicationClaims.Where(x => x.Selected);

        //            selectedClaim = (from cs in allClaim
        //                             from sV in selectedClaimValues
        //                             where cs.Value == sV.Value
        //                             select cs).ToList();

        //            var alreadyExistClaims = await _userManager.GetClaimsAsync(aUser);

        //            var toAddClaims = from s in selectedClaimValues
        //                              from a in alreadyExistClaims
        //                              where s.Value != a.Value
        //                              select s;

        //            var toRemoveClaims = from a in alreadyExistClaims
        //                                 from s in selectedClaimValues
        //                                 where a.Value != s.Value
        //                                 select a;
        //            foreach (var item in toRemoveClaims)
        //            {
        //                await _userManager.RemoveClaimAsync(aUser, item);
        //            }
        //            foreach (var item in selectedClaim)
        //            {
        //                await _userManager.AddClaimAsync(aUser, item);
        //            }
        //            ViewBag.UserName = aUser.UserName;
        //            var userIsInRole = await _userManager.GetRolesAsync(aUser);
        //            var userInClaims = await _userManager.GetClaimsAsync(aUser);
        //            model.UserRoleClaimsVM.ApplicationUser = aUser;
        //            Employee employee = await _employeeManager.GetByIdAsync(aUser.ReferenceId);
        //            model.Employee = employee;

        //            model.UserRoleClaimsVM.ApplicationRoles = await _roleManager.Roles.Select(x => new SelectListItem()
        //            {
        //                Text = x.Name,
        //                Value = x.Id,
        //                Selected = userIsInRole.Contains(x.Id)
        //            }).ToListAsync();

        //            model.UserRoleClaimsVM.ApplicationUser = aUser;
        //            model.UserRoleClaimsVM.ApplicationClaims = claimStore.Select(s => new SelectListItem
        //            {
        //                Text = s.ClaimType,
        //                Value = s.ClaimValue,
        //                Selected = userInClaims.Any(c => c.Value == s.ClaimValue)
        //            }).ToList();
        //            TempData["created"] = "User Profile updated";
        //        }
        //        catch (Exception)
        //        {
        //            TempData["failed"] = "Exception! User Profile Update Failed";
        //        }
        //    }
        //    model.Modules = modules;
        //    return View(model);
        //}

        public async Task<IActionResult> UserProfile(UserProfileVM model)
        {
            GlobalUI.PageTitle = "User Profile";

            var allUsers = await _userManager.Users
                .Where(u => u.UserType == 'e')
                .ToListAsync();

            var userId = model.UserRoleClaimsVM.ApplicationUser.Id;
            var aUser = await _userManager.FindByIdAsync(userId);
            if (aUser == null)
            {
                TempData["failed"] = "User not found.";
                return View(model);
            }

            ViewBag.UserList = model.UserList = new SelectList(allUsers, "Id", "UserName", aUser.Id);
            ViewBag.UserName = aUser.UserName;

            try
            {
                // Claims
                var claimStore = await _claimStoreManager.GetAllAsync();
                var allClaims = claimStore.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

                var selectedValues = model.UserRoleClaimsVM.ApplicationClaims
                    .Where(x => x.Selected)
                    .Select(x => x.Value)
                    .ToHashSet();

                var selectedClaims = allClaims
                    .Where(c => selectedValues.Contains(c.Value))
                    .ToList();

                var existingClaims = await _userManager.GetClaimsAsync(aUser);

                // Claims to remove
                var toRemove = existingClaims
                    .Where(ec => !selectedValues.Contains(ec.Value))
                    .ToList();

                // Claims to add
                var toAdd = selectedClaims
                    .Where(sc => !existingClaims.Any(ec => ec.Value == sc.Value))
                    .ToList();

                foreach (var claim in toRemove)
                    await _userManager.RemoveClaimAsync(aUser, claim);

                foreach (var claim in toAdd)
                    await _userManager.AddClaimAsync(aUser, claim);

                // Roles
                var userRoles = await _userManager.GetRolesAsync(aUser);

                model.UserRoleClaimsVM.ApplicationRoles = await _roleManager.Roles
                    .Select(role => new SelectListItem
                    {
                        Text = role.Name,
                        Value = role.Id,
                        Selected = userRoles.Contains(role.Id)
                    }).ToListAsync();

                var userClaims = await _userManager.GetClaimsAsync(aUser);
                model.UserRoleClaimsVM.ApplicationClaims = claimStore
                    .Select(c => new SelectListItem
                    {
                        Text = c.ClaimType,
                        Value = c.ClaimValue,
                        Selected = userClaims.Any(uc => uc.Value == c.ClaimValue)
                    }).ToList();

                // Employee info
                model.Employee = await _employeeManager.GetByIdAsync(aUser.ReferenceId);

                // Modules
                model.Modules = (List<ProjectModule>)await _projectModuleManager.GetAllAsync();

                model.UserRoleClaimsVM.ApplicationUser = aUser;
                TempData["created"] = "User Profile updated";
            }
            catch (Exception)
            {
                TempData["failed"] = "Exception! User Profile Update Failed";
            }

            return View(model);
        }

    }
}
