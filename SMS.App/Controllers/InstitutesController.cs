using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem;
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
        // GET: InstitutesController
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

        // GET: InstitutesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InstitutesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InstitutesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Institute institute, IFormFile logo, IFormFile banner)
        {
            try
            {
                if (logo != null)
                {
                    string logoName = "";
                    string root = _host.WebRootPath;
                    string folder = "/Images/Institute";
                    logoName = "instituteLogo_" + Guid.NewGuid() + Path.GetExtension(logo.FileName);
                    var pathCombine = Path.Combine(root, folder, logoName);
                    using var stream = new FileStream(pathCombine, FileMode.Create);
                    await logo.CopyToAsync(stream);
                    institute.Logo = logoName;
                }
                if (banner !=null)
                {
                    string banarName = "";
                    string root = _host.WebRootPath;
                    string folder = "/Images/Institiute";
                    banarName = "instituteBanner_" + Guid.NewGuid() + Path.GetExtension(logo.FileName);
                    var pathCombine = Path.Combine(root, folder, banarName);
                    using var stream = new FileStream(pathCombine, FileMode.Create);
                    institute.Banner = banarName;
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

        // GET: InstitutesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var institute = await _instituteManager.GetByIdAsync(id);
            return View(institute);
        }

        // POST: InstitutesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id,Institute existingInstitute, IFormFile logo, IFormFile banner)
        {
            string InstituteLogo = "";
            string InstituteBanner = "";
            if (ModelState.IsValid)
            {
                try
                {
                    if (logo != null)
                    {
                        string fileExt = Path.GetExtension(logo.FileName);
                        string root = _host.WebRootPath;
                        string folder = "Images/Institute/";
                        InstituteLogo = "instituteLogo_" + Guid.NewGuid() + fileExt;
                        string pathCombine = Path.Combine(root, folder, InstituteLogo);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await logo.CopyToAsync(stream);
                        }
                        existingInstitute.Logo = InstituteLogo;
                    }

                    if (banner != null)
                    {
                        string fileExt = Path.GetExtension(banner.FileName);
                        string root = _host.WebRootPath;
                        string folder = "Images/Institute/";
                        string fileName = "instituteBanner_" + Guid.NewGuid() + fileExt;
                        string pathCombine = Path.Combine(root, folder, fileName);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await banner.CopyToAsync(stream);
                        }
                        existingInstitute.Banner = InstituteBanner;
                    }

                    existingInstitute.EditedAt = DateTime.Now;
                    existingInstitute.EditedBy = HttpContext.Session.GetString("UserId");
                    await _instituteManager.UpdateAsync(existingInstitute);

                    return View();

                }
                catch
                {
                    return View();
                }
            }
            var institute = await _instituteManager.GetByIdAsync(id);
            return View(institute);
        }

        // GET: InstitutesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InstitutesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public ActionResult Setting()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Setting(string name)
        {
            GlobalUI.InstituteName = name;
            return View();
        }

    }
}
