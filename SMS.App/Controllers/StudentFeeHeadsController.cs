using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DB;
using SMS.Entities;

namespace SchoolManagementSystem.Controllers
{
    public class StudentFeeHeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentFeeHeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentFeeHeads
        public async Task<IActionResult> Index()
        {
            return View(await _context.StudentFeeHead.ToListAsync());
        }

        // GET: StudentFeeHeads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _context.StudentFeeHead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudentFeeHeads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Repeatedly,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            var sfhExist = _context.StudentFeeHead.FirstOrDefault(s => s.Name.Trim() == studentFeeHead.Name.Trim());
            if (sfhExist!=null)
            {
                msg = studentFeeHead.Name+" Fee Head is already exist.";
                ViewBag.msg = msg;
                TempData["crateFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    studentFeeHead.CreatedBy = HttpContext.Session.GetString("User");
                    studentFeeHead.CreatedAt = DateTime.Now;

                
                    _context.Add(studentFeeHead);
                    await _context.SaveChangesAsync();

                    TempData["Create"] = "Successfully Created";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _context.StudentFeeHead.FindAsync(id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }
            return View(studentFeeHead);
        }

        // POST: StudentFeeHeads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Repeatedly,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            if (id != studentFeeHead.Id)
            {
                return NotFound();
            }
            var sfhExist = _context.StudentFeeHead.FirstOrDefault(s => s.Name.Trim() == studentFeeHead.Name.Trim() && s.Id != studentFeeHead.Id);

            if (sfhExist!=null)
            {
                msg = studentFeeHead.Name + " is already exists";
                ViewBag.msg = msg;
                TempData["editFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        studentFeeHead.EditedAt = DateTime.Now;
                        studentFeeHead.EditedBy = HttpContext.Session.GetString("User");

                        _context.Update(studentFeeHead);
                        await _context.SaveChangesAsync();
                        TempData["edit"] = "Edited Successfully.";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!StudentFeeHeadExists(studentFeeHead.Id))
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
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _context.StudentFeeHead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // POST: StudentFeeHeads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentFeeHead = await _context.StudentFeeHead.FindAsync(id);
            _context.StudentFeeHead.Remove(studentFeeHead);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentFeeHeadExists(int id)
        {
            return _context.StudentFeeHead.Any(e => e.Id == id);
        }
    }
}
