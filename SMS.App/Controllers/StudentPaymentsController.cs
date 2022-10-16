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
using Microsoft.AspNetCore.Authorization;
using SMS.App.Utilities.MACIPServices;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class StudentPaymentsController : Controller
    {
        private readonly IStudentPaymentManager _studentPaymentManager;
        private readonly IStudentManager _studentManager;
        private readonly IClassFeeListManager _classFeeListManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IStudentFeeHeadManager _studentFeeHeadManager;

        public StudentPaymentsController(IStudentPaymentManager studentPaymentManager, IStudentManager studentManager, IClassFeeListManager classFeeListManager, IAcademicClassManager academicClassManager, IStudentFeeHeadManager studentFeeHeadManager)
        {
            _studentPaymentManager = studentPaymentManager;
            _studentManager = studentManager;
            _classFeeListManager = classFeeListManager;
            _academicClassManager = academicClassManager;
            _studentFeeHeadManager = studentFeeHeadManager;
        }

        // GET: StudentPayments
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var msg = "";
                if (TempData["success"] != null)
                {
                    msg = TempData["success"].ToString();
                }
                if (TempData["msg"] != null)
                {
                    msg = TempData["msg"].ToString();
                }
                ViewBag.msg = msg;
            }
            catch (Exception)
            {

                throw;
            }
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int? stRoll)
        {
            if (stRoll < 0 || stRoll == 0 || stRoll ==null)
            {
                
                return RedirectToAction("Index");
            }

            List<StudentPayment> studentPayments = new ();
            StudentPaymentVM spvm = new ();

            var student = await _studentManager.GetStudentByClassRollAsync((int)stRoll);
            if (student!=null)
            {
                StudentPayment sp = new();
                sp.Student = student;
               
                spvm.StudentPayment = sp;
                studentPayments = (List<StudentPayment>)await _studentPaymentManager.GetAllByStudentIdAsync(student.Id);
                spvm.StudentPayments = studentPayments.OrderBy(p => p.PaidDate).ToList();
                spvm.StudentId = student.Id;
                List<ClassFeeList> feeList = new();
                var classfeelist = await _classFeeListManager.GetAllByClassIdAsync(student.AcademicClassId);

                ViewBag.FeeList = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
                foreach (var item in classfeelist)
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

        [HttpPost]
        public async Task<IActionResult> Payment(StudentPaymentVM paymentObject)
        {
            try
            {
                paymentObject.StudentPayment.ReceiptNo = await GetReceiptNo(paymentObject.StudentPayment.StudentId, paymentObject.ClassFeeHeadId);
                StudentPayment studentPaymentObject = new StudentPayment();
                List<StudentPaymentDetails> studentPaymentDetailsObject = new List<StudentPaymentDetails>();
                studentPaymentObject.StudentId = paymentObject.StudentPayment.StudentId;
                studentPaymentObject.TotalPayment = paymentObject.StudentPayment.TotalPayment;
                studentPaymentObject.PaidDate = paymentObject.StudentPayment.PaidDate;
                studentPaymentObject.Remarks = paymentObject.StudentPayment.Remarks;
                if (paymentObject.StudentPayment.StudentPaymentDetails != null)
                {
                    foreach (var paymentDetails in paymentObject.StudentPayment.StudentPaymentDetails)
                    {
                        paymentDetails.CreatedAt = DateTime.Now;
                        paymentDetails.CreatedBy = HttpContext.Session.GetString("UserId");
                        paymentDetails.MACAddress = MACService.GetMAC();
                        studentPaymentDetailsObject.Add(paymentDetails);                 
                    }
                    studentPaymentObject.ReceiptNo = paymentObject.StudentPayment.ReceiptNo;
                    studentPaymentObject.CreatedAt = DateTime.Now;
                    studentPaymentObject.CreatedBy = HttpContext.Session.GetString("UserId");
                    studentPaymentObject.MACAddress = MACService.GetMAC();
                    studentPaymentObject.StudentPaymentDetails = studentPaymentDetailsObject;

                    bool isSaved = await _studentPaymentManager.AddAsync(studentPaymentObject);
                    if (isSaved)
                    {
                        TempData["success"] = ViewBag.msg = "New payment added successfully!";
                    }
                    else
                    {
                        TempData["fail"] = ViewBag.msg = "Failed to payment";
                        return RedirectToAction("Payment", new { id = paymentObject.StudentId });
                    }
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Payment", new { id = paymentObject.StudentId });
                throw;
            }
            
            return RedirectToAction("Index");
        }
        // GET: StudentPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _studentPaymentManager.GetByIdAsync((int)id);

            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // GET: StudentPayments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: StudentPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( StudentPaymentVM studentyPaymentVM, IFormFile waiverAttachment)
        {
            Student student = new();
            if (studentyPaymentVM != null)
            {
                student =await _studentManager.GetByIdAsync(studentyPaymentVM.StudentPayment.StudentId);
                
            }
            if (ModelState.IsValid)
            {
                studentyPaymentVM.StudentPayment.CreatedAt = DateTime.Now;
                studentyPaymentVM.StudentPayment.CreatedBy = HttpContext.Session.GetString("UserId");

                StudentPayment sPayment = new();
                sPayment = studentyPaymentVM.StudentPayment;

                if (studentyPaymentVM.StudentPayment.StudentPaymentDetails!=null)
                {
                    sPayment.StudentPaymentDetails = studentyPaymentVM.StudentPayment.StudentPaymentDetails;
                }

                bool isSaved = await _studentPaymentManager.AddAsync(sPayment);
                if (isSaved)
                {
                    TempData["Saved"] = "Successfully Saved";
                    return RedirectToAction("Payment", "StudentPayments", new { stRoll = student.ClassRoll });
                }
                
            }

            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentyPaymentVM.StudentPayment.StudentId);
            return RedirectToAction(nameof(Index));
        }

        // GET: StudentPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _studentPaymentManager.GetByIdAsync((int)id);
            if (studentPayment == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
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
                    studentPayment.EditedAt = DateTime.Now;
                    studentPayment.EditedBy = HttpContext.Session.GetString("UserId");

                    await _studentPaymentManager.UpdateAsync(studentPayment);
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
            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // GET: StudentPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _studentPaymentManager.GetByIdAsync((int)id);
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
            var studentPayment = await _studentPaymentManager.GetByIdAsync((int)id);
            await _studentPaymentManager.RemoveAsync(studentPayment);
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPaymentExists(int id)
        {
            var r = _studentPaymentManager.GetById(id);
            if (r!=null)
            {
                return true;
            }
            return false;

        }
        public async Task<double> GetTotalPayment(int stuRoll)
        {
            Student student = await _studentManager.GetStudentByClassRollAsync(stuRoll);
            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear, 01, 01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);
            return 0.0;
        }
        public async Task<double> GetCurrentDue(int stuRoll)
        {
            Student student = await _studentManager.GetStudentByClassRollAsync(stuRoll);

            var session = student.AcademicSession.Name.Trim().Split('-', 4);
            int sessionYear = Convert.ToInt32(session);

            DateTime startingDate = new DateTime(sessionYear,01,01);
            DateTime finalDate = new DateTime(sessionYear, 12, 31);

            return 0.0;
        }
        public async Task<string> GetReceiptNo(int studentId, int feeHeadId)
        {
            if (studentId >= 0)
            {
                Student stu = await _studentManager.GetByIdAsync(studentId);
                if (stu == null)
                {
                    return "";
                }
            }
            else
            {
                TempData["errorMsg"] = "Student Id is not provide due to Get Receipt No";
                return "";
            }
            var receiptNo = string.Empty;
            
            try
            {
                receiptNo = await _studentPaymentManager.GetNewReceipt(studentId, feeHeadId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return receiptNo;
        }
    }
}

