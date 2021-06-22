using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    public class DesignationsController : Controller
    {
        private readonly IDesignationManager _designatinManager;
        private readonly IDesignationTypeManager _designationTypeManager;
        private readonly IEmpTypeManager _empTypeManager;

        public DesignationsController(IDesignationManager designatinManager, IDesignationTypeManager designationTypeManager, IEmpTypeManager empTypeManager)
        {
            _designatinManager = designatinManager;
            _designationTypeManager = designationTypeManager;
            _empTypeManager = empTypeManager;
        }

        // GET: Designations
        public async Task<IActionResult> Index()
        {
            return View(await _designatinManager.GetAllAsync());
        }

        // GET: Designations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designatinManager.GetByIdAsync((int)id);
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        // GET: Designations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DesignationTypeList"] = new SelectList(await _designationTypeManager.GetAllAsync(), "Id", "DesignationTypeName");
            ViewData["EmpTypeList"] = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DesignationName,DesignationTypeId,EmpTypeId,CreatedBy,CreatedAt,EditedBy,EditedAt")] Designation designation)
        {
            
            if (ModelState.IsValid)
            {
                designation.CreatedAt = DateTime.Now;
                designation.CreatedBy = HttpContext.Session.GetString("User");

                await _designatinManager.AddAsync(designation);
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["DesignationTypeList"] = new SelectList(await _designationTypeManager.GetAllAsync(), "Id", "DesignationTypeName", designation.DesignationTypeId);
            ViewData["EmpTypeList"] = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name",designation.EmpTypeId);
            return View(designation);
        }

        // GET: Designations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designatinManager.GetByIdAsync((int)id);
            if (designation == null)
            {
                return NotFound();
            }

            ViewData["DesignationTypeList"] = new SelectList(await _designationTypeManager.GetAllAsync(), "Id", "DesignationTypeName",designation.DesignationTypeId);
            ViewData["EmpTypeList"] = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name", designation.EmpTypeId);
            return View(designation);
        }

        // POST: Designations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DesignationName,DesignationTypeId,EmpTypeId,CreatedBy,CreatedAt,EditedBy,EditedAt")] Designation designation)
        {
            if (id != designation.Id)
            {
                return NotFound();
            }

            
            if (ModelState.IsValid)
            {
                try
                {

                    designation.EditedAt = DateTime.Now;
                    designation.EditedBy = HttpContext.Session.GetString("User");

                    await _designatinManager.UpdateAsync(designation);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignationExists(designation.Id))
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

            ViewData["DesignationTypeList"] = new SelectList(await _designationTypeManager.GetAllAsync(), "Id", "DesignationTypeName", designation.DesignationTypeId);
            ViewData["EmpTypeList"] = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name", designation.EmpTypeId);
            return View(designation);
        }

        // GET: Designations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designatinManager.GetByIdAsync((int)id);
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        // POST: Designations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var designation = await _designatinManager.GetByIdAsync((int)id);

            await _designatinManager.RemoveAsync(designation);
            return RedirectToAction(nameof(Index));
        }

        private bool DesignationExists(int id)
        {
            var designation = _designatinManager.GetByIdAsync(id);
            if (designation!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("/api/designations/GetByEmpType")]
        public async Task<IReadOnlyCollection<Designation>> GetByEmpType(int id)
        {
            return await _designatinManager.GetByEmpType(id);
        }
    }
}
