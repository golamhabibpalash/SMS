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
using SMS.App.ViewModels;

namespace SMS.App.Controllers
{
    public class StudentPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentPayments
        [HttpGet]
        public IActionResult Index()
        {
            var msg = "";
            if (TempData["msg"]!=null)
            {
                msg = TempData["msg"].ToString();
            }
            ViewBag.msg = msg;
            return View();
        }        
        
        public async Task<IActionResult> Payment(int? stRoll)
        {
            if (stRoll < 0 || stRoll == 0 || stRoll ==null)
            {
                return RedirectToAction("Index");
            }

            List<StudentPayment> studentPayments = new ();
            StudentyPaymentVM spvm = new ();

            var student =await _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicSection)
                .FirstOrDefaultAsync(s => s.ClassRoll == stRoll);
            if (student!=null)
            {
                StudentPayment sp = new();
                sp.Student = student;
                spvm.StudentPayment = sp;
                studentPayments = await _context.StudentPayment.Where(s => s.StudentId == student.Id).ToListAsync();
                spvm.StudentPayments = studentPayments;

                List<ClassFeeList> feeList = new();
                var fList = from cfl in _context.ClassFeeList.Include(c => c.StudentFeeHead)
                            from s in _context.Student.Where(s => s.ClassRoll == stRoll)
                            from ac in _context.AcademicClass
                            where cfl.AcademicClassId == ac.Id && s.AcademicClassId == ac.Id
                            select cfl;
                foreach (var item in fList)
                {
                    feeList.Add(item);
                }
                spvm.ClassFeeLists = feeList;
                ViewBag.roll = stRoll;
                return View(spvm);
            }
            else
            {
                TempData["msg"] = "Student Not Found";
                return RedirectToAction("Index");
            }
            
        }

        // GET: StudentPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayment
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // GET: StudentPayments/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name");
            return View();
        }

        // POST: StudentPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( StudentyPaymentVM studentyPaymentVM, IFormFile waiverAttachment)
        {
            Student student = new();
            if (studentyPaymentVM != null)
            {
                student = _context.Student.FirstOrDefault(s => s.Id == studentyPaymentVM.StudentPayment.StudentId);
                
            }
            if (ModelState.IsValid)
            {
                studentyPaymentVM.StudentPayment.CreatedAt = DateTime.Now;
                studentyPaymentVM.StudentPayment.CreatedBy = HttpContext.Session.GetString("User");

                StudentPayment sPayment = new();
                sPayment = studentyPaymentVM.StudentPayment;

                if (studentyPaymentVM.StudentPayment.StudentPaymentDetails!=null)
                {
                    sPayment.StudentPaymentDetails = studentyPaymentVM.StudentPayment.StudentPaymentDetails;
                }


                _context.StudentPayment.Add(sPayment);
                await _context.SaveChangesAsync();
                TempData["Saved"] = "Successfully Saved";
                return RedirectToAction("Payment", "StudentPayments", new { stRoll = student.ClassRoll });
            }

            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", studentyPaymentVM.StudentPayment.StudentId);
            return RedirectToAction(nameof(Index));
        }

        // GET: StudentPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayment.FindAsync(id);
            if (studentPayment == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // POST: StudentPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,TotalPayment,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentPayment studentPayment)
        {
            if (id != studentPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentPaymentExists(studentPayment.Id))
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
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // GET: StudentPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayment
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // POST: StudentPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentPayment = await _context.StudentPayment.FindAsync(id);
            _context.StudentPayment.Remove(studentPayment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPaymentExists(int id)
        {
            return _context.StudentPayment.Any(e => e.Id == id);
        }
        public double GetTotalPayment(int stuRoll)
        {
            Student student = _context.Student.Include(s => s.AcademicSession).FirstOrDefault(s => s.ClassRoll == stuRoll);
            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear, 01, 01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);
            return 0.0;
        }
        public double GetCurrentDue(int stuRoll)
        {
            Student student = _context.Student.Include(s => s.AcademicSession).FirstOrDefault(s => s.ClassRoll == stuRoll);

            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear,01,01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);

            return 0.0;
        }
    }
}
