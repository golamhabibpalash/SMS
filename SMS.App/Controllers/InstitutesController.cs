using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.InstituteVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class InstitutesController : Controller
    {
        private readonly IInstituteManager _instituteManager;
        private readonly IWebHostEnvironment _host;

        public InstitutesController(IInstituteManager instituteManager, IWebHostEnvironment host)
        {
            _instituteManager = instituteManager;
            _host = host;
        }
        

        public async Task<ActionResult> Index()
        {
            var result = await _instituteManager.GetAllAsync();
            if (result.Count() != 0)
            {
                var instituteInfo = result.FirstOrDefault();
                if (instituteInfo != null)
                {
                    return View(instituteInfo);
                }
            }
            
            return RedirectToAction("Create");
        }


        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Institute institute, IFormFile logo, IFormFile banner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (banner != null)
                    {
                        string banarName = "";
                        string root = _host.WebRootPath;
                        string folder = "Images/Institute";
                        banarName = "instituteBanner_" + Guid.NewGuid() + Path.GetExtension(banner.FileName);
                        var pathCombine = Path.Combine(root, folder, banarName);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await banner.CopyToAsync(stream);
                        }
                            institute.Banner = banarName;
                    }
                    if (logo != null)
                    {
                        string logoName = "";
                        string root = _host.WebRootPath;
                        string folder = "Images/Institute";
                        logoName = "instituteLogo_" + Guid.NewGuid() + Path.GetExtension(logo.FileName);
                        var pathCombine = Path.Combine(root, folder, logoName);
                        
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await logo.CopyToAsync(stream);
                        }
                        institute.Logo = logoName;
                    }

                    institute.CreatedAt = DateTime.Now;
                    institute.CreatedBy = HttpContext.Session.GetString("UserId");

                    await _instituteManager.AddAsync(institute);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View(institute);
            }
            
        }


        public async Task<ActionResult> Edit(int id)
        {
            var institute = await _instituteManager.GetByIdAsync(id);
            return View(institute);
        }


        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id,Institute existingInstitute, IFormFile logo, IFormFile banner, IFormFile fav_Icon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string img = "";
                    string root = _host.WebRootPath;
                    string folder = "Images/Institute/";
                    if (logo != null)
                    {
                        string fileExt = Path.GetExtension(logo.FileName);
                        img = "instituteLogo_" + Guid.NewGuid() + fileExt;
                        string pathCombine = Path.Combine(root, folder, img);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await logo.CopyToAsync(stream);
                        }
                        existingInstitute.Logo = img;
                    }

                    if (banner != null)
                    {
                        string fileExt = Path.GetExtension(banner.FileName);
                        img = "instituteBanner_" + Guid.NewGuid() + fileExt;
                        string pathCombine = Path.Combine(root, folder, img);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await banner.CopyToAsync(stream);
                        }
                        existingInstitute.Banner = img;
                    }

                    if (fav_Icon !=null)
                    {
                        string fileExt = Path.GetExtension(fav_Icon.FileName);
                        img = "instituteFavIcon_" + Guid.NewGuid() + fileExt;
                        string pathCombine = Path.Combine(root, folder, img);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await fav_Icon.CopyToAsync(stream);
                        }
                        existingInstitute.FavIcon = img;
                    }
                    existingInstitute.EditedAt = DateTime.Now;
                    existingInstitute.EditedBy = HttpContext.Session.GetString("UserId");
                    existingInstitute.MACAddress = MACService.GetMAC();
                    bool isUpdate = await _instituteManager.UpdateAsync(existingInstitute);
                    if (isUpdate)
                    {
                        TempData["updated"] = "Information updated successfully";
                    }
                    else
                    {
                        TempData["error"] = "Failed to update.";
                    }
                }
                catch
                {
                    return View(existingInstitute);
                }
            }
            else
            {
                TempData["error"] = "Input value something wrong.";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> SchoolTimeTable()
        {
            Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
            InstituteTimeVM instituteTimeVM = new InstituteTimeVM() { };
            instituteTimeVM.StartingTime = institute.StartingTime;
            instituteTimeVM.ClosingTime = institute.ClosingTime;
            instituteTimeVM.LateTimeStart = institute.LateTime;

            return View(instituteTimeVM);
        }

        [HttpPost]
        public async Task<ActionResult> SchoolTimeTable(InstituteTimeVM model)
        {
            if (ModelState.IsValid)
            {
                Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
                if (institute != null)
                {
                    DateOnly dateOnly = new DateOnly(1900, 01, 01);
                    string amPmIndicator = model.StartingTime.ToString("tt", System.Globalization.CultureInfo.InvariantCulture);
                    institute.StartingTime = model.StartingTime;
                    if (amPmIndicator=="pm")
                    {
                        institute.StartingTime.AddHours(12);
                    }
                    amPmIndicator = model.ClosingTime.ToString("tt", System.Globalization.CultureInfo.InvariantCulture);
                    institute.ClosingTime = model.ClosingTime;
                    if (amPmIndicator.ToLower()=="pm")
                    {
                        institute.ClosingTime.AddHours(12);
                    }

                    TimeSpan span = model.LateTimeStart-model.StartingTime;

                    institute.LateTime = model.LateTimeStart;

                    await _instituteManager.UpdateAsync(institute);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }


    }
}
