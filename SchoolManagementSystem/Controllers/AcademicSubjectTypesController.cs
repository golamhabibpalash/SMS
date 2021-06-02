using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DatabaseContext;
using Models;

namespace SchoolManagementSystem.Controllers
{
    public class AcademicSubjectTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicSubjectTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AcademicSubjectTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.AcademicSubjectType.ToListAsync());
        }

        // GET: AcademicSubjectTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubjectType = await _context.AcademicSubjectType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }

            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AcademicSubjectTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectTypeId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubjectType academicSubjectType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicSubjectType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubjectType = await _context.AcademicSubjectType.FindAsync(id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }
            return View(academicSubjectType);
        }

        // POST: AcademicSubjectTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectTypeId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubjectType academicSubjectType)
        {
            if (id != academicSubjectType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicSubjectType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicSubjectTypeExists(academicSubjectType.Id))
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
            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubjectType = await _context.AcademicSubjectType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }

            return View(academicSubjectType);
        }

        // POST: AcademicSubjectTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicSubjectType = await _context.AcademicSubjectType.FindAsync(id);
            _context.AcademicSubjectType.Remove(academicSubjectType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSubjectTypeExists(int id)
        {
            return _context.AcademicSubjectType.Any(e => e.Id == id);
        }
    }
}
