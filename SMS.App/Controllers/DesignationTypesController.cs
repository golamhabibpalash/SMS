using SMS.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using SMS.BLL.Contracts;

namespace SchoolManagementSystem.Controllers
{
    public class DesignationTypesController : Controller
    {
        private readonly IDesignationTypeManager _designationTypeManager;

        public DesignationTypesController(IDesignationTypeManager designationTypeManager)
        {
            _designationTypeManager = designationTypeManager;
        }

        // GET: DesignationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _designationTypeManager.GetAllAsync());
        }

        // GET: DesignationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designationType = await _designationTypeManager.GetByIdAsync((int)id);
            if (designationType == null)
            {
                return NotFound();
            }

            return View(designationType);
        }

        // GET: DesignationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DesignationTypeName,CreatedBy,CreatedAt,EditedBy,EditedAt")] DesignationType designationType)
        {

            if (ModelState.IsValid)
            {
                designationType.CreatedAt = DateTime.Now;
                designationType.CreatedBy = HttpContext.Session.GetString("User");

                await _designationTypeManager.AddAsync(designationType);
                return RedirectToAction(nameof(Index));
            }
            return View(designationType);
        }

        // GET: DesignationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designationType = await _designationTypeManager.GetByIdAsync((int)id);
            if (designationType == null)
            {
                return NotFound();
            }
            return View(designationType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DesignationTypeName,CreatedBy,CreatedAt,EditedBy,EditedAt")] DesignationType designationType)
        {
            if (id != designationType.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    designationType.EditedAt = DateTime.Now;
                    designationType.EditedBy = HttpContext.Session.GetString("User");

                    await _designationTypeManager.UpdateAsync(designationType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignationTypeExists(designationType.Id))
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
            return View(designationType);
        }

        // GET: DesignationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designationType = await _designationTypeManager.GetByIdAsync((int)id);
            if (designationType == null)
            {
                return NotFound();
            }

            return View(designationType);
        }

        // POST: DesignationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var designationType = await _designationTypeManager.GetByIdAsync(id);
            await _designationTypeManager.RemoveAsync(designationType);
            return RedirectToAction(nameof(Index));
        }

        private bool DesignationTypeExists(int id)
        {
            var designationType = _designationTypeManager.GetByIdAsync(id);

            if (designationType !=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
