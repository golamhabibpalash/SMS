using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DB;
using SMS.Entities;

namespace SchoolManagementSystem.Controllers
{
    public class AcademicSectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicSectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AcademicSections
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AcademicSection.Include(a => a.AcademicClass);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AcademicSections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSection = await _context.AcademicSection
                .Include(a => a.AcademicClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicSection == null)
            {
                return NotFound();
            }

            return View(academicSection);
        }

        // GET: AcademicSections/Create
        public IActionResult Create()
        {
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name");
            return View();
        }

        // POST: AcademicSections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSection academicSection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicSection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", academicSection.AcademicClassId);
            return View(academicSection);
        }

        // GET: AcademicSections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSection = await _context.AcademicSection.FindAsync(id);
            if (academicSection == null)
            {
                return NotFound();
            }
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", academicSection.AcademicClassId);
            return View(academicSection);
        }

        // POST: AcademicSections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSection academicSection)
        {
            if (id != academicSection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicSection);
                    await _context.SaveChangesAsync();
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
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", academicSection.AcademicClassId);
            return View(academicSection);
        }

        // GET: AcademicSections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSection = await _context.AcademicSection
                .Include(a => a.AcademicClass)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var academicSection = await _context.AcademicSection.FindAsync(id);
            _context.AcademicSection.Remove(academicSection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSectionExists(int id)
        {
            return _context.AcademicSection.Any(e => e.Id == id);
        }
    }
}
