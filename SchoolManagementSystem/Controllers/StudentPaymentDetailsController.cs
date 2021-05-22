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
    public class StudentPaymentDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentPaymentDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentPaymentDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentPaymentDetails.Include(s => s.StudentFeeHead).Include(s => s.StudentPayment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentPaymentDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPaymentDetails = await _context.StudentPaymentDetails
                .Include(s => s.StudentFeeHead)
                .Include(s => s.StudentPayment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPaymentDetails == null)
            {
                return NotFound();
            }

            return View(studentPaymentDetails);
        }

        // GET: StudentPaymentDetails/Create
        public IActionResult Create()
        {
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Id");
            ViewData["StudentPaymentId"] = new SelectList(_context.StudentPayment, "Id", "Id");
            return View();
        }

        // POST: StudentPaymentDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentPaymentId,StudentFeeHeadId,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentPaymentDetails studentPaymentDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentPaymentDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Id", studentPaymentDetails.StudentFeeHeadId);
            ViewData["StudentPaymentId"] = new SelectList(_context.StudentPayment, "Id", "Id", studentPaymentDetails.StudentPaymentId);
            return View(studentPaymentDetails);
        }

        // GET: StudentPaymentDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPaymentDetails = await _context.StudentPaymentDetails.FindAsync(id);
            if (studentPaymentDetails == null)
            {
                return NotFound();
            }
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Id", studentPaymentDetails.StudentFeeHeadId);
            ViewData["StudentPaymentId"] = new SelectList(_context.StudentPayment, "Id", "Id", studentPaymentDetails.StudentPaymentId);
            return View(studentPaymentDetails);
        }

        // POST: StudentPaymentDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentPaymentId,StudentFeeHeadId,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentPaymentDetails studentPaymentDetails)
        {
            if (id != studentPaymentDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentPaymentDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentPaymentDetailsExists(studentPaymentDetails.Id))
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
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Id", studentPaymentDetails.StudentFeeHeadId);
            ViewData["StudentPaymentId"] = new SelectList(_context.StudentPayment, "Id", "Id", studentPaymentDetails.StudentPaymentId);
            return View(studentPaymentDetails);
        }

        // GET: StudentPaymentDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPaymentDetails = await _context.StudentPaymentDetails
                .Include(s => s.StudentFeeHead)
                .Include(s => s.StudentPayment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPaymentDetails == null)
            {
                return NotFound();
            }

            return View(studentPaymentDetails);
        }

        // POST: StudentPaymentDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentPaymentDetails = await _context.StudentPaymentDetails.FindAsync(id);
            _context.StudentPaymentDetails.Remove(studentPaymentDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPaymentDetailsExists(int id)
        {
            return _context.StudentPaymentDetails.Any(e => e.Id == id);
        }
    }
}
