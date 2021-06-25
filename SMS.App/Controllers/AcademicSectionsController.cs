using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    public class AcademicSectionsController : Controller
    {
        private readonly IAcademicSectionManager _academicSectionManager;
        private readonly IAcademicClassManager _academicClassManager;

        private readonly IAcademicSessionManager _academicSessionManager;

        public AcademicSectionsController(IAcademicSectionManager academicSectionManager, IAcademicClassManager academicClassManager, IAcademicSessionManager academicSessionManager)
        {
            _academicSectionManager = academicSectionManager;
            _academicClassManager = academicClassManager;
            _academicSessionManager = academicSessionManager;
        }

        // GET: AcademicSections
        public async Task<IActionResult> Index()
        {
            var aSectiion = await _academicSectionManager.GetAllAsync();
            return View(aSectiion);
        }

        // GET: AcademicSections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = Convert.ToInt32(id);
            var academicSection = await _academicSectionManager.GetByIdAsync(myId);
            if (academicSection == null)
            {
                return NotFound();
            }

            return View(academicSection);
        }

        // GET: AcademicSections/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: AcademicSections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,AcademicClassId,AcademicSessionId,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSection academicSection)
        {
            if (ModelState.IsValid)
            {
                await _academicSectionManager.AddAsync(academicSection);
                return RedirectToAction(nameof(Index));
            }
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicSection.AcademicClassId);
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", academicSection.AcademicSessionId);
            return View(academicSection);
        }

        // GET: AcademicSections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = Convert.ToInt32(id);
            var academicSection = await _academicSectionManager.GetByIdAsync(myId);
            
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicSection.AcademicClassId);
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", academicSection.AcademicSessionId);
            return View(academicSection);
        }

        // POST: AcademicSections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,AcademicClassId,AcademicSessionId,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSection academicSection)
        {
            if (id != academicSection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _academicSectionManager.UpdateAsync(academicSection);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicSectionExists(academicSection.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicSection.AcademicClassId);
            ViewData["AcademicSessionId"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", academicSection.AcademicSessionId);
            return View(academicSection);
        }

        // GET: AcademicSections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = Convert.ToInt32(id);
            var academicSection = await _academicSectionManager.GetByIdAsync(myId);
            if (academicSection == null)
            {
                return NotFound();
            }
            return View(academicSection);
        }

        // POST: AcademicSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicSection = await _academicSectionManager.GetByIdAsync(id);
            await _academicSectionManager.RemoveAsync(academicSection);
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSectionExists(int id)
        {
            var isExist = _academicSectionManager.GetByIdAsync(id);
            if (isExist !=null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [Route("api/academicsections/getbyclasswithsessionId")]
        public async Task<IReadOnlyCollection<AcademicSection>> GetAllByClassWithSessionId(int classId, int sessionId)
        {
            return await _academicSectionManager.GetAllByClassWithSessionId(classId, sessionId);
        }
    }
}
