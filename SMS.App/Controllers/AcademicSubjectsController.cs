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
    public class AcademicSubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AcademicSubjects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AcademicSubject.Include(a => a.AcademicSubjectType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AcademicSubjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _context.AcademicSubject
                .Include(a => a.AcademicSubjectType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicSubject == null)
            {
                return NotFound();
            }

            return View(academicSubject);
        }

        // GET: AcademicSubjects/Create
        public IActionResult Create()
        {
            ViewData["AcademicSubjectTypeId"] = new SelectList(_context.Set<AcademicSubjectType>(), "Id", "Id");
            return View();
        }

        // POST: AcademicSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectName,AcademicSubjectTypeId,TotalMarks,IsOptional,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubject academicSubject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AcademicSubjectTypeId"] = new SelectList(_context.Set<AcademicSubjectType>(), "Id", "Id", academicSubject.AcademicSubjectTypeId);
            return View(academicSubject);
        }

        // GET: AcademicSubjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _context.AcademicSubject.FindAsync(id);
            if (academicSubject == null)
            {
                return NotFound();
            }
            ViewData["AcademicSubjectTypeId"] = new SelectList(_context.Set<AcademicSubjectType>(), "Id", "Id", academicSubject.AcademicSubjectTypeId);
            return View(academicSubject);
        }

        // POST: AcademicSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectName,AcademicSubjectTypeId,TotalMarks,IsOptional,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubject academicSubject)
        {
            if (id != academicSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicSubjectExists(academicSubject.Id))
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
            ViewData["AcademicSubjectTypeId"] = new SelectList(_context.Set<AcademicSubjectType>(), "Id", "Id", academicSubject.AcademicSubjectTypeId);
            return View(academicSubject);
        }

        // GET: AcademicSubjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _context.AcademicSubject
                .Include(a => a.AcademicSubjectType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicSubject == null)
            {
                return NotFound();
            }

            return View(academicSubject);
        }

        // POST: AcademicSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicSubject = await _context.AcademicSubject.FindAsync(id);
            _context.AcademicSubject.Remove(academicSubject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSubjectExists(int id)
        {
            return _context.AcademicSubject.Any(e => e.Id == id);
        }
    }
}
