using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class InstitutesController : Controller
    {
        private readonly IInstituteManager _instituteManager;

        public InstitutesController(IInstituteManager instituteManager)
        {
            _instituteManager = instituteManager;
        }
        // GET: InstitutesController
        public async Task<ActionResult> Index()
        {

            return View(await _instituteManager.GetAllAsync());
            
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
        public async Task<ActionResult> Create(Institute institute)
        {
            try
            {
                
                await _instituteManager.AddAsync(institute);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InstitutesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InstitutesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
    }
}
