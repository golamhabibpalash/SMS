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
using SMS.BLL.Contracts;

namespace SMS.App.Controllers
{
    public class StudentPaymentsController : Controller
    {
        private readonly IStudentPaymentManager studentPaymentManager;
        private readonly IStudentManager studentManager;
        private readonly IClassFeeListManager classFeeListManager;
        private readonly IAcademicClassManager academicClassManager;

        public StudentPaymentsController(IStudentPaymentManager studentPaymentManager, IStudentManager studentManager, IClassFeeListManager classFeeListManager, IAcademicClassManager academicClassManager)
        {
            this.studentPaymentManager = studentPaymentManager;
            this.studentManager = studentManager;
            this.classFeeListManager = classFeeListManager;
            this.academicClassManager = academicClassManager;
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

            var student = await studentManager.GetStudentByClassRollAsync((int)stRoll);
            if (student!=null)
            {
                StudentPayment sp = new();
                sp.Student = student;
                spvm.StudentPayment = sp;
                studentPayments = (List<StudentPayment>)await studentPaymentManager.GetAllByStudentIdAsync(student.Id);
                spvm.StudentPayments = studentPayments;

                List<ClassFeeList> feeList = new();
                var classfeelist = await classFeeListManager.GetAllAsync();
                var academicClasses = await academicClassManager.GetAllAsync();
                var fList = from cfl in classfeelist
                            from ac in academicClasses
                            where cfl.AcademicClassId == ac.Id && student.AcademicClassId == ac.Id
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

            var studentPayment = await studentPaymentManager.GetByIdAsync((int)id);

            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // GET: StudentPayments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await studentManager.GetAllAsync(), "Id", "Name");
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
                student =await studentManager.GetByIdAsync(studentyPaymentVM.StudentPayment.StudentId);
                
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

                bool isSaved = await studentPaymentManager.AddAsync(sPayment);
                if (isSaved)
                {
                    TempData["Saved"] = "Successfully Saved";
                    return RedirectToAction("Payment", "StudentPayments", new { stRoll = student.ClassRoll });
                }
                
            }

            ViewData["StudentId"] = new SelectList(await studentManager.GetAllAsync(), "Id", "Name", studentyPaymentVM.StudentPayment.StudentId);
            return RedirectToAction(nameof(Index));
        }

        // GET: StudentPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await studentPaymentManager.GetByIdAsync((int)id);
            if (studentPayment == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(await studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // POST: StudentPayments/Edit/5
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
                    await studentPaymentManager.UpdateAsync(studentPayment);
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
            ViewData["StudentId"] = new SelectList(await studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // GET: StudentPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await studentPaymentManager.GetByIdAsync((int)id);
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
            var studentPayment = await studentPaymentManager.GetByIdAsync((int)id);
            await studentPaymentManager.RemoveAsync(studentPayment);
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPaymentExists(int id)
        {
            var r = studentPaymentManager.GetById(id);
            if (r!=null)
            {
                return true;
            }
            return false;

        }
        public async Task<double> GetTotalPayment(int stuRoll)
        {
            Student student = await studentManager.GetStudentByClassRollAsync(stuRoll);
            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear, 01, 01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);
            return 0.0;
        }
        public async Task<double> GetCurrentDue(int stuRoll)
        {
            Student student = await studentManager.GetStudentByClassRollAsync(stuRoll);

            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear,01,01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);

            return 0.0;
        }
    }
}
