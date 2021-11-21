﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SMS.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.Entities;
using Microsoft.AspNetCore.Authorization;

namespace SMS.App.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _contex;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(ApplicationDbContext contex, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _contex = contex;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            string msg = "";
            if (TempData["msg"]!=null)
            {
                
                msg = TempData["msg"].ToString();

            }
            ViewBag.msg = msg;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string rname)
        {
            string msg = "";

            if (!string.IsNullOrEmpty(rname))
            {
                bool exist = await _roleManager.RoleExistsAsync(rname);
                if (!exist)
                {
                    IdentityRole role = new IdentityRole(rname);
                    await _roleManager.CreateAsync(role);
                    msg = "Role [" + rname + "] has been created successfully.";
                }
                else
                {
                    msg = "Role [" + rname + "] is already exist.";
                    ViewBag.msg = msg;
                }
            }
            else
            {
                msg = "Please Input a name first.";
            }

            TempData["msg"] = msg;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            string msg = "";
            if (TempData["msg"]!=null)
            {
                msg = TempData["msg"].ToString();
            }
            var userList = _userManager.Users;
            var roleList = _roleManager.Roles;

            ViewBag.users = userList;
            ViewBag.roles = roleList;
            ViewBag.msg = msg;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AssignRole(string appUser, string appRole)
        {
            string msg = "";
            if (!string.IsNullOrEmpty(appUser))
            {
                if (!string.IsNullOrEmpty(appRole))
                {
                    IdentityUser user = await _userManager.FindByEmailAsync(appUser);
                    if (user!=null)
                    {
                        bool existRole =await _userManager.IsInRoleAsync(user, appRole);
                        if (existRole)
                        {
                            msg = "Role is already assigned to this user.";
                        }
                        else
                        {
                           await _userManager.AddToRoleAsync(user, appRole);
                            msg = "Role has been assigned to this user";
                        }
                    }
                    
                }
                else
                {
                    msg = "Please select a role from list";
                }
            }
            else
            {
                msg = "Please Select a User from list";
            }
            TempData["msg"] = msg;

            return RedirectToAction("AssignRole");
        }

    }
}
