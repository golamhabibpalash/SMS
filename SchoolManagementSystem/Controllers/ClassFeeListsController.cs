using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DatabaseContext;
using Models;

namespace SchoolManagementSystem.Controllers
{
    public class ClassFeeListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassFeeListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentFeeLists
        public async Task<IActionResult> Index()
        {
            string msg = "";
            if (TempData["success"]!=null)
            {
                msg = TempData["success"].ToString();
            }
            var result =await _context.ClassFeeList
                .Include(s => s.StudentFeeHead)
                .Include(s => s.AcademicClass)
                .OrderBy(s => s.AcademicClass.ClassSerial)
                .ThenBy(s => s.StudentFeeHead.Name)
                .ToListAsync();
            ViewBag.msg = msg;
            return View(result);
        }

        // GET: StudentFeeLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeList = await _context.ClassFeeList
                .Include(s => s.AcademicClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentFeeList == null)
            {
                return NotFound();
            }

            return View(studentFeeList);
        }

        // GET: StudentFeeLists/Create
        public IActionResult Create()
        {
            
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Name");
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name");
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            return View();
        }

        // POST: StudentFeeLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            string msg = "";

            var feeListExist =await _context.ClassFeeList
                .FirstOrDefaultAsync(s =>   s.AcademicClassId == classFeeList.AcademicClassId &&
                                            s.StudentFeeHeadId==classFeeList.StudentFeeHeadId);

            if (feeListExist!=null)
            {
                msg = "Fee list for this class is already exists.";
                TempData["crateFail"] = msg;
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    classFeeList.CreatedAt = DateTime.Now;
                    classFeeList.CreatedBy = HttpContext.Session.GetString("UserId");

                    msg = "Saved Successfully.";
                    _context.Add(classFeeList);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = msg;
                    TempData["create"] = msg;
                    return RedirectToAction(nameof(Index));
                }
            }


            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Name",classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", classFeeList.AcademicClassId);

            return View(classFeeList);
        }

        // GET: StudentFeeLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classFeeList = await _context.ClassFeeList.FindAsync(id);
            if (classFeeList == null)
            {
                return NotFound();
            }
            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Name", classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", classFeeList.AcademicClassId);
            return View(classFeeList);
        }

        // POST: StudentFeeLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            if (id != classFeeList.Id)
            {
                return NotFound();
            }
            var feeListExist = await _context.ClassFeeList
                .FirstOrDefaultAsync(s => s.AcademicClassId == classFeeList.AcademicClassId && s.Id !=id);
            if (feeListExist!=null)
            {
                TempData["editFail"] = "Failed";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        classFeeList.EditedAt = DateTime.Now;
                        classFeeList.EditedBy = HttpContext.Session.GetString("UserId");

                        _context.Update(classFeeList);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!StudentFeeListExists(classFeeList.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["edit"] = "Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }


            ViewData["StudentFeeHeadId"] = new SelectList(_context.StudentFeeHead, "Id", "Name",classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(_context.AcademicClass, "Id", "Name", classFeeList.AcademicClassId);
            return View(classFeeList);
        }

        // GET: StudentFeeLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeList = await _context.ClassFeeList
                .Include(s => s.AcademicClass)
                .Include(s => s.StudentFeeHead)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentFeeList == null)
            {
                return NotFound();
            }

            return View(studentFeeList);
        }

        // POST: StudentFeeLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentFeeList = await _context.ClassFeeList.FindAsync(id);
            _context.ClassFeeList.Remove(studentFeeList);
            await _context.SaveChangesAsync();
            TempData["delete"] = "Successfully Deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool StudentFeeListExists(int id)
        {
            return _context.ClassFeeList.Any(e => e.Id == id);
        }
    }
}
