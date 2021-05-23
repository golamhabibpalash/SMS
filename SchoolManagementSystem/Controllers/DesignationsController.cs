using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class DesignationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DesignationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Designations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Designation.Include(s => s.DesignationType).ToListAsync());
        }

        // GET: Designations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        // GET: Designations/Create
        public IActionResult Create()
        {
            ViewData["DesignationTypeList"] = new SelectList(_context.DesignationType, "Id", "DesignationTypeName");
            return View();
        }

        // POST: Designations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DesignationName,DesignationTypeId,CreatedBy,CreatedAt,EditedBy,EditedAt")] Designation designation)
        {
            string msg = "";
            var isExist = await _context.Designation.FirstOrDefaultAsync(d => d.DesignationName.Trim() == designation.DesignationName.Trim());
            if (isExist!=null)
            {
                msg = designation.DesignationName + " is already exist.";
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    designation.CreatedAt = DateTime.Now;
                    designation.CreatedBy = HttpContext.Session.GetString("User");

                    _context.Add(designation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["DesignationTypeList"] = new SelectList(_context.DesignationType, "Id", "DesignationTypeName", designation.DesignationTypeId);
            return View(designation);
        }

        // GET: Designations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designation.FindAsync(id);
            if (designation == null)
            {
                return NotFound();
            }

            ViewData["DesignationTypeList"] = new SelectList(_context.DesignationType, "Id", "DesignationTypeName");
            return View(designation);
        }

        // POST: Designations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DesignationName,DesignationTypeId,CreatedBy,CreatedAt,EditedBy,EditedAt")] Designation designation)
        {
            if (id != designation.Id)
            {
                return NotFound();
            }

            string msg = "";
            var isExist = await _context.Designation.FirstOrDefaultAsync(d => d.DesignationName.Trim() == designation.DesignationName.Trim() && d.Id != designation.Id);
            if (isExist != null)
            {
                msg = designation.DesignationName + " is already exist.";
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        designation.EditedAt = DateTime.Now;
                        designation.EditedBy = HttpContext.Session.GetString("User");

                        _context.Update(designation);
                        await _context.SaveChangesAsync();
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

            }

            ViewData["DesignationTypeList"] = new SelectList(_context.DesignationType, "Id", "DesignationTypeName", designation.DesignationTypeId);
            return View(designation);
        }

        // GET: Designations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designation
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var designation = await _context.Designation.FindAsync(id);
            _context.Designation.Remove(designation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignationExists(int id)
        {
            return _context.Designation.Any(e => e.Id == id);
        }
    }
}
