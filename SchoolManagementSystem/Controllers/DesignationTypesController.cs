using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class DesignationTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DesignationTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DesignationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DesignationType.ToListAsync());
        }

        // GET: DesignationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designationType = await _context.DesignationType
                .FirstOrDefaultAsync(m => m.Id == id);
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

        // POST: DesignationTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DesignationTypeName,CreatedBy,CreatedAt,EditedBy,EditedAt")] DesignationType designationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(designationType);
                await _context.SaveChangesAsync();
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

            var designationType = await _context.DesignationType.FindAsync(id);
            if (designationType == null)
            {
                return NotFound();
            }
            return View(designationType);
        }

        // POST: DesignationTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(designationType);
                    await _context.SaveChangesAsync();
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

            var designationType = await _context.DesignationType
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var designationType = await _context.DesignationType.FindAsync(id);
            _context.DesignationType.Remove(designationType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignationTypeExists(int id)
        {
            return _context.DesignationType.Any(e => e.Id == id);
        }
    }
}
