using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using SMS.BLL.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SMS.App.Controllers
{
    [Authorize]
    public class AcademicClassesController : Controller
    {
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly ILogger<AcademicClassesController> _Logger;

        public AcademicClassesController(IAcademicClassManager academicClassManager, IAcademicSessionManager academicSessionManager, ILogger<AcademicClassesController> Logger)
        {
            _academicClassManager = academicClassManager;
            _academicSessionManager = academicSessionManager;
            _Logger = Logger;
        }

        // GET: AcademicClasses
        public async Task<IActionResult> Index()
        {
            var AcademicClassList =await _academicClassManager.GetAllAsync();

            return View(AcademicClassList);
        }

        // GET: AcademicClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicClass = await _academicClassManager.GetByIdAsync(Convert.ToInt32(id));
            if (academicClass == null)
            {
                return NotFound();
            }

            return View(academicClass);
        }

        // GET: AcademicClasses/Create
        public async Task<IActionResult> Create()
        {
            var allSession = await _academicSessionManager.GetAllAsync();
            ViewData["AcademicSessionId"] = new SelectList(allSession, "Id", "Name");
            return View();
        }

        // POST: AcademicClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AcademicSessionId,ClassSerial,Description,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicClass academicClass)
        {
            string msg = "";
            if (academicClass.Name!=null)
            {
                if (ModelState.IsValid)
                {
                    academicClass.CreatedBy = HttpContext.Session.GetString("UserId");
                    academicClass.CreatedAt = DateTime.Today;

                    var isSaved = await _academicClassManager.AddAsync(academicClass);
                    if (isSaved)
                    {
                        TempData["create"] = "Created Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        msg = "Save Fail! "+academicClass.Name+" is already exist!";
                    }
                }
            }
            else
            {
                msg = "Please insert a class name.";
            }
            ViewBag.msg = msg;
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name");
            return View(academicClass);
        }

        // GET: AcademicClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicClass = await _academicClassManager.GetByIdAsync(Convert.ToInt32(id));
            if (academicClass == null)
            {
                return NotFound();
            }
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name");
            return View(academicClass);
        }

        // POST: AcademicClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AcademicSessionId,ClassSerial,Description,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicClass academicClass)
        {
            string msg = "";
            if (id != academicClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    academicClass.EditedBy = HttpContext.Session.GetString("UserId");
                    academicClass.EditedAt = DateTime.Now;
                    await _academicClassManager.UpdateAsync(academicClass);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( !AcademicClassExists(academicClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["edit"] = "Updated Successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.msg = msg;
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name");
            return View(academicClass);
        }

        // GET: AcademicClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var academicClass = await _academicClassManager.GetByIdAsync(Convert.ToInt32(id));

            if (academicClass == null)
            {
                return NotFound();
            }

            return View(academicClass);
        }

        // POST: AcademicClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicClass = await _academicClassManager.GetByIdAsync(id);
            await _academicClassManager.RemoveAsync(academicClass);
            TempData["delete"] = "Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicClassExists(int id)
        {
            var aClass = _academicClassManager.GetByIdAsync(id);
            if (aClass!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("/api/academicClass/getAll")]
        public async Task<IReadOnlyCollection<AcademicClass>> GetAll()
        {
            return await _academicClassManager.GetAllAsync();
        }
    }
}
