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
using SMS.App.Utilities.ShortMessageService;

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
        private readonly IStudentPaymentDetailsManager _studentPaymentDetailsManager;
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        private readonly IPhoneSMSManager _phoneSMSManager;
        private readonly IInstituteManager _instituteManager;
        private readonly IAcademicSessionManager _academicSessionManager;

        public StudentPaymentsController(IStudentPaymentManager studentPaymentManager, IStudentManager studentManager, IClassFeeListManager classFeeListManager, IAcademicClassManager academicClassManager, IStudentFeeHeadManager studentFeeHeadManager, IStudentPaymentDetailsManager studentPaymentDetailsManager, ISetupMobileSMSManager setupMobileSMSManager, IPhoneSMSManager phoneSMSManager, IInstituteManager instituteManager, IAcademicSessionManager academicSessionManager)
        {
            _studentPaymentManager = studentPaymentManager;
            _studentManager = studentManager;
            _classFeeListManager = classFeeListManager;
            _academicClassManager = academicClassManager;
            _studentFeeHeadManager = studentFeeHeadManager;
            _studentPaymentDetailsManager = studentPaymentDetailsManager;
            _setupMobileSMSManager = setupMobileSMSManager;
            _phoneSMSManager = phoneSMSManager;
            _instituteManager = instituteManager;
            _academicSessionManager = academicSessionManager;
        }

        // GET: StudentPayments
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var msg = "";
                if (TempData["success"] != null)
                {
                    msg = TempData["success"].ToString();
                    TempData["created"] = msg;
                }
                if (TempData["msg"] != null)
                {
                    msg = TempData["msg"].ToString();
                }
                ViewBag.msg = msg;
                ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(),"Id","Name");
            }
            catch (Exception)
            {

                throw;
            }
            
            return View();
        }
        public IActionResult PaymentNew()
        {
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
            var inst = await _instituteManager.GetFirstOrDefaultAsync();
            ViewBag.InstituteName = inst.Name;
            var student = await _studentManager.GetStudentByClassRollAsync((int)stRoll);
            if (student!=null)
            {
                StudentPayment sp = new();
                StudentPaymentDetails details = new();
                sp.Student = student;
                List<StudentPaymentDetails> studentPaymentDetails = new();
                sp.StudentPaymentDetails = studentPaymentDetails;
                sp.StudentPaymentDetails.Add(details);
                spvm.StudentPayment = sp;
                studentPayments = (List<StudentPayment>)await _studentPaymentManager.GetAllByStudentIdAsync(student.Id);
                spvm.StudentPayments = studentPayments.OrderBy(p => p.PaidDate).ToList();
                spvm.StudentId = student.Id;
                List<ClassFeeList> feeList = new();
                var classfeelist = await _classFeeListManager.GetAllByClassIdAsync(student.AcademicClassId);
                List<StudentFeeHead> feeHeadList = (List<StudentFeeHead>)await _studentFeeHeadManager.GetAllAsync();
                AcademicSession currentSession = await _academicSessionManager.GetCurrentAcademicSession();

                feeHeadList = (from f in feeHeadList
                               join t in classfeelist on f.Id equals t.StudentFeeHeadId
                               where t.AcademicSessionId == currentSession.Id
                               select f).ToList();


                ViewData["FeeList"] = new SelectList(feeHeadList, "Id", "Name");
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

                ViewData["FeeList"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
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
                        TempData["Saved"] = ViewBag.msg = "New payment added successfully!";
                        if (paymentObject.IsSMSSend == true)
                        {
                            var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
                            if (smsSetup.SMSService == true)
                            {
                                Student studentObject = await _studentManager.GetByIdAsync(paymentObject.StudentPayment.StudentId);
                                foreach (var item in studentPaymentDetailsObject)
                                {
                                    if (item.PaidAmount<=0)
                                    {
                                        item.PaidAmount = paymentObject.StudentPayment.TotalPayment;
                                    }
                                    StudentFeeHead feeHead = await _studentFeeHeadManager.GetByIdAsync(item.StudentFeeHeadId);
                                    string smsText = studentObject.Name + " has Paid " + item.PaidAmount + "Tk as " + feeHead.Name + " -Noble";
                                    string phoneNo = studentObject.GuardianPhone;
                                    bool isSend = await MobileSMS.SendSMS(smsText, phoneNo);
                                    if (isSend)
                                    {
                                        PhoneSMS sms = new PhoneSMS();
                                        sms.SMSType = "payment";
                                        sms.MACAddress = MACService.GetMAC();
                                        sms.Text = smsText;
                                        sms.MobileNumber = phoneNo;
                                        sms.CreatedAt = DateTime.Now;
                                        sms.CreatedBy = HttpContext.Session.GetString("UserId");

                                        await _phoneSMSManager.AddAsync(sms);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["fail"] = ViewBag.msg = "Failed to payment";
                    }
                }

            }
            catch (Exception)
            {
              throw;
            }
            Student student = await _studentManager.GetByIdAsync(paymentObject.StudentPayment.StudentId);
            return RedirectToAction("Payment", new { stRoll = student.ClassRoll });
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
            var classfeelist = await _classFeeListManager.GetAllByClassIdAsync(studentPayment.Student.AcademicClassId);
            List<StudentFeeHead> feeHeadList = (List<StudentFeeHead>)await _studentFeeHeadManager.GetAllAsync();
            AcademicSession currentSession = await _academicSessionManager.GetCurrentAcademicSession();

            feeHeadList = (from f in feeHeadList
                           join t in classfeelist on f.Id equals t.StudentFeeHeadId
                           where t.AcademicSessionId == currentSession.Id
                           select f).ToList();

            ViewData["FeeList"] = new SelectList(feeHeadList, "Id", "Name", studentPayment.StudentPaymentDetails[0].StudentFeeHeadId);
            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
            return View(studentPayment);
        }

        // POST: StudentPayments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentPayment studentPayment)
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
                    studentPayment.MACAddress = MACService.GetMAC();
                    foreach (var item in studentPayment.StudentPaymentDetails)
                    {
                        item.EditedAt = DateTime.Now;
                        item.EditedBy = HttpContext.Session.GetString("UserId");
                        item.MACAddress = MACService.GetMAC();
                        item.StudentPaymentId = studentPayment.Id;
                        item.StudentPayment = studentPayment;
                        bool isPaymentDetailsUpdated = await _studentPaymentDetailsManager.UpdateAsync(item);
                    }
                    studentPayment.Student = await _studentManager.GetByIdAsync(studentPayment.StudentId);

                    bool isUpdated = await _studentPaymentManager.UpdateAsync(studentPayment);
                    if (isUpdated)
                    {
                        TempData["updated"] = "Payment updated successfull";                        
                    }
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
            }
            else
            {
                ViewData["FeeList"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name", studentPayment.StudentPaymentDetails[0].StudentFeeHeadId);
                ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentPayment.StudentId);
            }
                return RedirectToAction("Payment", new { stRoll = studentPayment.Student.ClassRoll});
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
            catch (Exception)
            {
                throw;
            }
            return receiptNo;
        }
    }
}

