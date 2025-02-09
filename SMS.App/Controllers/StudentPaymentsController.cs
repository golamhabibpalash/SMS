using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem;
using SMS.App.Utilities.MACIPServices;
using SMS.App.Utilities.Others;
using SMS.App.Utilities.ShortMessageService;
using SMS.App.ViewModels;
using SMS.App.ViewModels.PaymentVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SessionWisePaymentDetails = SMS.Entities.AdditionalModels.SessionWisePaymentDetails;
using SessionWisePaymentVM = SMS.Entities.AdditionalModels.SessionWisePaymentVM;
using SinglePaymentVM = SMS.Entities.AdditionalModels.SinglePaymentVM;

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
        private readonly IAcademicSectionManager _academicSectionManager;
        private readonly IStudentFeeAllocationManager _studentFeeAllocationManager;
        private readonly HttpClient _httpClient;

        public StudentPaymentsController(IStudentPaymentManager studentPaymentManager, IStudentManager studentManager, IClassFeeListManager classFeeListManager, IAcademicClassManager academicClassManager, IStudentFeeHeadManager studentFeeHeadManager, IStudentPaymentDetailsManager studentPaymentDetailsManager, ISetupMobileSMSManager setupMobileSMSManager, IPhoneSMSManager phoneSMSManager, IInstituteManager instituteManager, IAcademicSessionManager academicSessionManager, IAcademicSectionManager academicSectionManager, IStudentFeeAllocationManager studentFeeAllocationManager, HttpClient httpClient)
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
            _academicSectionManager = academicSectionManager;
            _studentFeeAllocationManager = studentFeeAllocationManager;
            _httpClient = httpClient;
        }

        // GET: StudentPayments
        [HttpGet]
        [Authorize(Policy = "IndexStudentPaymentsPolicy")]
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
                if (TempData["studentNotFound"] != null)
                {
                    msg = TempData["studentNotFound"].ToString();
                    TempData["failed"] = msg;
                }
                ViewBag.msg = msg;
                ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        [HttpGet]
        [Authorize(Policy = "PaymentStudentPaymentsPolicy")]
        public async Task<IActionResult> Payment(int? stRoll)
        {
            ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            if (stRoll < 0 || stRoll == 0 || stRoll == null)
            {
                return RedirectToAction("Index");
            }
            var msg = "";
            if (TempData["success"] != null)
            {
                msg = TempData["success"].ToString();
                TempData["created"] = msg;
            }
            ViewBag.msg = msg;
            List<StudentPayment> studentPayments = new();
            StudentPaymentVM spvm = new();
            spvm.CurrentAcademicSession = await _academicSessionManager.GetCurrentAcademicSession();
            var inst = await _instituteManager.GetFirstOrDefaultAsync();
            ViewBag.InstituteName = inst.Name;
            var stu = await _studentManager.GetStudentByClassRollAsync((int)stRoll);
            if (stu == null)
            {
                TempData["studentNotFound"] = "Student Not Found";
                return RedirectToAction("Index");
            }
            try
            {
                var student = await _studentManager.GetStudentByUniqueIdAsync(Convert.ToInt32(stu.UniqueId).ToString());
                if (student != null)
                {
                    StudentPayment sp = new();
                    StudentPaymentDetails details = new();
                    sp.Student = student;
                    List<StudentPaymentDetails> studentPaymentDetails = new();
                    sp.StudentPaymentDetails = studentPaymentDetails;
                    sp.StudentPaymentDetails.Add(details);
                    spvm.StudentPayment = sp;
                    spvm.StudentPreviousPayments = (List<StudentPayment>)await _studentPaymentManager.GetAllByStudentIdAsync(student.Id);
                    spvm.StudentCurrentPayments = (List<StudentPayment>)await _studentPaymentManager.GetAllByStudentIdAsync(student.Id);
                    spvm.StudentId = student.Id;
                    List<ClassFeeList> feeList = new();
                    var classfeelist = await _classFeeListManager.GetAllByClassIdAsync(student.AcademicClassId);
                    classfeelist = classfeelist.Where(s => s.AcademicSessionId == student.AcademicSessionId).ToList();

                    List<StudentFeeHead> feeHeadList = (List<StudentFeeHead>)await _studentFeeHeadManager.GetAllAsync();
                    AcademicSession currentSession = spvm.CurrentAcademicSession;

                    feeHeadList = (from f in feeHeadList
                                   join t in classfeelist on f.Id equals t.StudentFeeHeadId
                                   where t.AcademicSessionId == student.AcademicSessionId
                                   select f).ToList();
                    if (student.IsResidential)
                    {
                        feeHeadList = feeHeadList.Where(s => s.IsResidential).ToList();
                    }
                    else
                    {
                        feeHeadList = feeHeadList.Where(s => s.IsResidential == false).ToList();
                    }

                    ViewData["FeeList"] = new SelectList(feeHeadList.OrderBy(s => s.SL), "Id", "Name");
                    AcademicSession academicSession = await _academicSessionManager.GetCurrentAcademicSession();
                    foreach (var item in classfeelist.Where(s => s.AcademicSessionId == currentSession.Id))
                    {
                        if (item.AcademicSessionId == academicSession.Id)
                        {
                            feeList.Add(item);
                        }
                    }
                    spvm.ClassFeeLists = feeList;
                    ViewBag.roll = stRoll;

                    StudentPaymentDetailVM studentPaymentDetailVM = await _studentPaymentManager.GetAllDetailPaymentByUniqueId(student.UniqueId);
                    List<SinglePaymentVM> singlePaymentVMs = new List<SinglePaymentVM>();
                    #region dummy data
                    SinglePaymentVM singlePaymentVM = new SinglePaymentVM();
                    singlePaymentVM.AcademicSession = "2024-2025";
                    singlePaymentVM.TotalAmount = 4800;
                    singlePaymentVM.TotalPaidAmount = 2800;
                    singlePaymentVM.TotalDueAmount = 2000;
                    if (feeHeadList.Count > 0)
                    {
                        foreach (var fHeadItem in feeHeadList.OrderBy(s => s.SL))
                        {
                            SessionWisePaymentVM sessionWisePaymentVM = new SessionWisePaymentVM()
                            {
                                FeeHeadName = fHeadItem.Name,
                                Amount = 0,
                                PaidAmount = 0,
                                Balance = 0,
                                Status = "Null"
                            };
                            singlePaymentVM.SessionWisePaymentVMs.Add(sessionWisePaymentVM);
                        }
                    }

                    SessionWisePaymentVM sessionWisePaymentVM1 = new SessionWisePaymentVM()
                    {
                        FeeHeadName = "January Monthly Fee",
                        Amount = 1600,
                        PaidAmount = 1600,
                        Balance = 0,
                        Status = "Paid"
                    };

                    SessionWisePaymentDetails sessionWisePaymentDetails1a = new SessionWisePaymentDetails()
                    {
                        PaidDate = new DateTime(2025, 1, 05).ToString("dd MMM yyyy"),
                        ReceiptNo = "5624178",
                        PayableAmount = 1600,
                        PaidAmount = 600,
                        DueAmount = 1000,
                        Status = "Partial"
                    };
                    sessionWisePaymentVM1.SessionWisePaymentDetails.Add(sessionWisePaymentDetails1a);

                    SessionWisePaymentDetails sessionWisePaymentDetails1b = new SessionWisePaymentDetails()
                    {
                        PaidDate = new DateTime(2025, 1, 10).ToString("dd MMM yyyy"),
                        ReceiptNo = "5624199",
                        PayableAmount = 1600,
                        PaidAmount = 1000,
                        DueAmount = 600,
                        Status = "Partial"
                    };
                    sessionWisePaymentVM1.SessionWisePaymentDetails.Add(sessionWisePaymentDetails1b);
                    singlePaymentVM.SessionWisePaymentVMs.Add(sessionWisePaymentVM1);

                    SessionWisePaymentVM sessionWisePaymentVM2 = new SessionWisePaymentVM()
                    {
                        FeeHeadName = "Februery Monthly Fee",
                        Amount = 1600,
                        PaidAmount = 1200,
                        Balance = 400,
                        Status = "Partial Paid"
                    };

                    SessionWisePaymentDetails sessionWisePaymentDetails2a = new SessionWisePaymentDetails()
                    {
                        PaidDate = new DateTime(2025, 2, 04).ToString("dd MMM yyyy"),
                        ReceiptNo = "5625034",
                        PayableAmount = 1600,
                        PaidAmount = 1000,
                        DueAmount = 600,
                        Status = "Partial"
                    };
                    sessionWisePaymentVM2.SessionWisePaymentDetails.Add(sessionWisePaymentDetails2a);

                    SessionWisePaymentDetails sessionWisePaymentDetails2b = new SessionWisePaymentDetails()
                    {
                        PaidDate = new DateTime(2025, 2, 05).ToString("dd MMM yyyy"),
                        ReceiptNo = "5625055",
                        PayableAmount = 600,
                        PaidAmount = 200,
                        DueAmount = 400,
                        Status = "Partial"
                    };
                    sessionWisePaymentVM2.SessionWisePaymentDetails.Add(sessionWisePaymentDetails2b);
                    singlePaymentVM.SessionWisePaymentVMs.Add(sessionWisePaymentVM2);

                    SessionWisePaymentVM sessionWisePaymentVM3 = new SessionWisePaymentVM()
                    {
                        FeeHeadName = "March Monthly Fee",
                        Amount = 1600,
                        PaidAmount = 0,
                        Balance = 1600,
                        Status = "Not Paid"
                    };
                    singlePaymentVM.SessionWisePaymentVMs.Add(sessionWisePaymentVM3);
                    singlePaymentVMs.Add(singlePaymentVM);
                    #endregion dummy data
                    //studentPaymentDetailVM.Payments = singlePaymentVMs;
                    spvm.PaymentVM = studentPaymentDetailVM ?? new StudentPaymentDetailVM();
                    //var existingAllPayments = await _studentPaymentDetailsManager.GetAllByStudentUniqueId(student.UniqueId);

                    return View(spvm);
                }
                else
                {
                    TempData["msg"] = "Student Not Found";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<List<SinglePaymentVM>> GetSinglePaymentList(string studentUniqueId)
        {
            List<SinglePaymentVM> singlePaymentVMs = new List<SinglePaymentVM>();
            var allSession = await _academicSessionManager.GetAllAsync();
            var allFeeHead = await _studentFeeHeadManager.GetAllAsync();
            if (allSession != null && allSession.Count > 0)
            {
                foreach (var session in allSession.OrderByDescending(s => s.Name))
                {
                    var singlePayment = new SinglePaymentVM
                    {
                        AcademicSession = session.Name,
                        PaymentsTitle = "Payment status and details",
                        SessionWisePaymentVMs = await GetSessionWisePaymentVMs(session.Id, studentUniqueId),
                    };
                    singlePaymentVMs.Add(singlePayment);
                }
            }
            return singlePaymentVMs;
        }
        private async Task<List<SessionWisePaymentVM>> GetSessionWisePaymentVMs(int academicSessionId, string studentUniqueId)
        {
            List<SessionWisePaymentVM> sessionWisePaymentVMs = new();
            var student = await _studentManager.GetStudentByUniqueIdAsync(studentUniqueId);
            var classFees = await _classFeeListManager.GetAllByClassIdAsync(student.AcademicClassId);
            classFees = classFees.Where(s => s.AcademicSessionId == academicSessionId && s.StudentFeeHead.IsResidential == student.IsResidential).ToList();
            if (classFees != null)
            {
                foreach (var item in classFees.OrderBy(s => s.StudentFeeHead.SL))
                {
                    var pAmount = await GetPaidAmount(studentUniqueId, academicSessionId, item.StudentFeeHeadId);
                    SessionWisePaymentVM sessionWisePaymentVM = new()
                    {
                        FeeHeadName = item.StudentFeeHead.Name,
                        Amount = item.Amount,
                        PaidAmount = pAmount,
                        Balance = pAmount - item.Amount,
                        Status = pAmount == item.Amount ? "Paid" : pAmount < item.Amount ? "Patial Paid" : "Unpaid",
                    };
                    sessionWisePaymentVMs.Add(sessionWisePaymentVM);
                }
            }
            return sessionWisePaymentVMs;
        }

        private async Task<double> GetPaidAmount(string studentUniqueId, int aSessionId, int feeHeadId)
        {
            var paidAmount = 0.0;
            var student = await _studentManager.GetStudentByUniqueIdAsync(studentUniqueId);
            int isResidential = student.IsResidential ? 1 : 0;
            var result = await _studentPaymentManager.GetPaidAmountByFeeHeadAsync(studentUniqueId, aSessionId, isResidential, student.AcademicClassId, feeHeadId);
            if (result.Count > 0)
            {
                paidAmount = Convert.ToDouble(result.FirstOrDefault().PaidAmount);
            }
            return paidAmount;
        }

        [HttpPost]
        [Authorize(Policy = "PaymentStudentPaymentsPolicy")]
        public async Task<IActionResult> Payment(StudentPaymentVM paymentObject)
        {
            paymentObject.CurrentAcademicSession = await _academicSessionManager.GetCurrentAcademicSession();
            try
            {
                paymentObject.StudentPayment.ReceiptNo = await GetReceiptNo(paymentObject.StudentPayment.StudentId, paymentObject.ClassFeeHeadId);
                StudentPayment studentPaymentObject = new StudentPayment();
                List<StudentPaymentDetails> studentPaymentDetailsObject = new List<StudentPaymentDetails>();
                studentPaymentObject.StudentId = paymentObject.StudentPayment.StudentId;
                studentPaymentObject.TotalPayment = paymentObject.StudentPayment.TotalPayment;
                studentPaymentObject.PaidDate = paymentObject.StudentPayment.PaidDate;
                studentPaymentObject.Remarks = paymentObject.StudentPayment.Remarks;
                studentPaymentObject.AcademicSessionId = paymentObject.CurrentAcademicSession.Id;
                //studentPaymentObject.AcademicSession = paymentObject.CurrentAcademicSession;
                studentPaymentObject.UniqueId = await _studentManager.GetUniqueIdByStudentId(paymentObject.StudentPayment.StudentId);
                var feeList = await _studentFeeHeadManager.GetAllAsync();
                ViewData["FeeList"] = new SelectList(feeList.OrderBy(s => s.SL), "Id", "Name");

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
                        if (paymentObject.IsSMSSend == true)
                        {
                            var smsSetup = await _setupMobileSMSManager.GetByIdAsync(1);
                            if (smsSetup.SMSService == true)
                            {
                                Student studentObject = await _studentManager.GetByIdAsync(paymentObject.StudentPayment.StudentId);
                                foreach (var item in studentPaymentDetailsObject)
                                {
                                    if (item.PaidAmount <= 0)
                                    {
                                        item.PaidAmount = paymentObject.StudentPayment.TotalPayment;
                                    }
                                    StudentFeeHead feeHead = await _studentFeeHeadManager.GetByIdAsync(item.StudentFeeHeadId);
                                    var instituteInfo = await _instituteManager.GetAllAsync();

                                    string smsText = studentObject.Name + " has Paid " + item.PaidAmount + "Tk as " + feeHead.Name + " -" + instituteInfo.FirstOrDefault().Name;
                                    string phoneNo = studentObject.GuardianPhone;
                                    bool isSend = await MobileSMS.SendSMS(phoneNo, smsText);
                                    if (isSend)
                                    {
                                        PhoneSMS sms = new PhoneSMS();
                                        sms.SMSType = "payment";
                                        sms.MACAddress = MACService.GetMAC();
                                        sms.Text = smsText;
                                        sms.MobileNumber = phoneNo;
                                        sms.CreatedAt = DateTime.Now;
                                        sms.CreatedBy = HttpContext.Session.GetString("UserId");
                                        sms.EditedAt = DateTime.Now;
                                        sms.EditedBy = HttpContext.Session.GetString("UserId");

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
        [Authorize(Policy = "CreateStudentPaymentsPolicy")]
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
        [Authorize(Policy = "CreateStudentPaymentsPolicy")]
        public async Task<IActionResult> Create(StudentPaymentVM studentyPaymentVM, IFormFile waiverAttachment)
        {
            Student student = new();
            if (studentyPaymentVM != null)
            {
                student = await _studentManager.GetByIdAsync(studentyPaymentVM.StudentPayment.StudentId);

            }
            if (ModelState.IsValid)
            {
                try
                {
                    studentyPaymentVM.StudentPayment.CreatedAt = DateTime.Now;
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                    {
                        return RedirectToAction("login", "Accounts");
                    }
                    studentyPaymentVM.StudentPayment.CreatedBy = HttpContext.Session.GetString("UserId");

                    StudentPayment sPayment = new();
                    sPayment = studentyPaymentVM.StudentPayment;

                    if (studentyPaymentVM.StudentPayment.StudentPaymentDetails != null)
                    {
                        sPayment.StudentPaymentDetails = studentyPaymentVM.StudentPayment.StudentPaymentDetails;
                    }

                    sPayment.MACAddress = MACService.GetMAC();
                    bool isSaved = await _studentPaymentManager.AddAsync(sPayment);
                    if (isSaved)
                    {
                        TempData["Saved"] = "Successfully Saved";
                        return RedirectToAction("Payment", "StudentPayments", new { stRoll = student.ClassRoll });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            ViewData["StudentId"] = new SelectList(await _studentManager.GetAllAsync(), "Id", "Name", studentyPaymentVM.StudentPayment.StudentId);
            return RedirectToAction(nameof(Index));
        }

        // GET: StudentPayments/Edit/5

        [Authorize(Policy = "EditStudentPaymentsPolicy")]
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
        [Authorize(Policy = "EditStudentPaymentsPolicy")]
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
                    StudentPayment existingStudentPayment = await _studentPaymentManager.GetByIdAsync(id);
                    existingStudentPayment.EditedAt = DateTime.Now;
                    existingStudentPayment.EditedBy = HttpContext.Session.GetString("UserId");
                    existingStudentPayment.TotalPayment = studentPayment.StudentPaymentDetails.Sum(s => s.PaidAmount);
                    existingStudentPayment.PaidDate = studentPayment.PaidDate;
                    existingStudentPayment.MACAddress = MACService.GetMAC();
                    existingStudentPayment.Student = studentPayment.Student = await _studentManager.GetByIdAsync(studentPayment.StudentId);
                    bool isUpdated = await _studentPaymentManager.UpdateAsync(existingStudentPayment);
                    if (isUpdated)
                    {
                        foreach (var item in studentPayment.StudentPaymentDetails)
                        {
                            StudentPaymentDetails existingDetails = await _studentPaymentDetailsManager.GetByIdAsync(item.Id);
                            existingDetails.EditedAt = DateTime.Now;
                            existingDetails.EditedBy = HttpContext.Session.GetString("UserId");
                            existingDetails.MACAddress = MACService.GetMAC();
                            existingDetails.StudentPaymentId = studentPayment.Id;
                            existingDetails.PaidAmount = item.PaidAmount;
                            existingDetails.StudentPayment = studentPayment;
                            await _studentPaymentDetailsManager.UpdateAsync(existingDetails);
                        }
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
            return RedirectToAction("Payment", new { stRoll = studentPayment.Student.ClassRoll });
        }
        public IActionResult Page()
        {
            return View();
        }
        // GET: StudentPayments/Delete/5
        [Authorize(Policy = "DeleteStudentPaymentsPolicy")]
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
        [Authorize(Policy = "DeleteStudentPaymentsPolicy")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentPayment = await _studentPaymentManager.GetByIdAsync((int)id);
            await _studentPaymentManager.RemoveAsync(studentPayment);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "DuePaymentStudentPaymentsPolicy")]
        public async Task<IActionResult> DuePayment()
        {
            GlobalUI.PageTitle = "Due Payment List";

            DuePaymentVM duePaymentVM = new DuePaymentVM();
            var classes = await _academicClassManager.GetAllAsync();
            duePaymentVM.AcademicClassList = new SelectList(classes.Where(s => s.Status == true), "Id", "Name").ToList();
            duePaymentVM.StudentStatusSelectList = new SelectList(GetActiveInActiveList(), "Id", "sName").ToList();
            var studentCategory = new List<SelectListItem>
            {
                new() { Text = "all", Value = "all" },
                new() { Text = "residential", Value = "residential" },
                new() { Text = "nonResidential", Value = "nonResidential" }
            };
            duePaymentVM.StudentCategorySelectList = new SelectList(studentCategory.ToList(), "Value", "Text").ToList();
            ViewBag.isFromPost = false;

            return View(duePaymentVM);
        }

        [HttpPost]
        [Authorize(Policy = "DuePaymentStudentPaymentsPolicy")]
        public async Task<IActionResult> DuePayment(int? aSessionId, int? AcademicClassId, int? AcademicSectionId, int studentId, int dueType, string isResidential, string status)
        {
            GlobalUI.PageTitle = "Due Payment List";
            if (string.IsNullOrEmpty(aSessionId.ToString()))
            {
                AcademicSession currentSession = await _academicSessionManager.GetCurrentAcademicSession();
                aSessionId = currentSession.Id;
            }
            List<Student> students = new List<Student>();
            if (string.IsNullOrEmpty(AcademicSectionId.ToString()))
            {
                AcademicSectionId = 0;
            }
            if (string.IsNullOrEmpty(AcademicClassId.ToString()))
            {
                AcademicClassId = 0;
            }

            students = await _studentManager.GetStudentsByClassSessionSectionAsync((int)aSessionId, (int)AcademicClassId, (int)AcademicSectionId);
            if (!string.IsNullOrEmpty(isResidential))
            {
                if (isResidential == "residential")
                {
                    students = students.Where(s => s.IsResidential == true).ToList();
                }
                if (isResidential == "nonResidential")
                {

                    students = students.Where(s => s.IsResidential == false).ToList();
                }
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "1")
                {
                    students = students.Where(s => s.Status == true).ToList();
                }

                if (status == "0")
                {
                    students = students.Where(s => s.Status == false).ToList();
                }
            }

            DuePaymentVM duePaymentVM = new DuePaymentVM();
            if (students != null)
            {
                duePaymentVM.ShowCount = students.Count;
            }
            var classes = await _academicClassManager.GetAllAsync();
            duePaymentVM.AcademicClassList = new SelectList(classes.Where(s => s.Status == true), "Id", "Name", AcademicClassId).ToList();
            duePaymentVM.AcademicSectionList = new SelectList(await _academicSectionManager.GetAllByClassWithSessionId((int)AcademicClassId, (int)aSessionId), "Id", "Name", duePaymentVM.AcademicSectionId).ToList(); duePaymentVM.StudentStatusSelectList = new SelectList(GetActiveInActiveList(), "Id", "sName", status).ToList();
            var studentCategory = new List<SelectListItem>
            {
                new() { Text = "all", Value = "all" },
                new() { Text = "residential", Value = "residential" },
                new() { Text = "nonResidential", Value = "nonResidential" }
            };
            duePaymentVM.StudentCategorySelectList = new SelectList(studentCategory.ToList(), "Value", "Text", isResidential).ToList();
            duePaymentVM.AcademicClassId = (int)AcademicClassId;
            duePaymentVM.AcademicClass = await _academicClassManager.GetByIdAsync((int)AcademicClassId);
            duePaymentVM.Institute = await _instituteManager.GetFirstOrDefaultAsync();
            duePaymentVM.GrandTotal = 0.00;
            List<DuePaymentDetailsVM> duePaymentDetailsVMs = new List<DuePaymentDetailsVM>();
            if (students != null)
            {
                foreach (var item in students)
                {
                    DuePaymentDetailsVM duePaymentDetailsVM = new DuePaymentDetailsVM();
                    duePaymentDetailsVM.StudentId = item.Id;
                    duePaymentDetailsVM.Student = item;
                    duePaymentDetailsVM.TotalDue = await _studentPaymentManager.GetStudentCurrentDue(item.Id);
                    duePaymentDetailsVMs.Add(duePaymentDetailsVM);
                }
            }
            duePaymentVM.DuePayments = duePaymentDetailsVMs;
            duePaymentVM.GrandTotal = duePaymentDetailsVMs.Sum(d => d.TotalDue);
            ViewBag.isFromPost = true;
            return View(duePaymentVM);
        }


        [HttpGet]
        [Authorize(Policy = "PreviousDuePaymentStudentPaymentsPolicy")]
        public async Task<IActionResult> DuePaymentPrevious()
        {
            GlobalUI.PageTitle = "Previous Due Payment List";
            DuePaymentVM previousDuePaymentVM = new()
            {
                AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList()
            };
            ViewBag.isFromPost = false;
            return View(previousDuePaymentVM);
        }

        private bool StudentPaymentExists(int id)
        {
            var r = _studentPaymentManager.GetById(id);
            if (r != null)
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

            DateTime startingDate = new(sessionYear, 01, 01);
            DateTime finalDate = new(sessionYear, 12, 31);
            return 0.0;
        }
        //public async Task<double> GetCurrentDue(int stuRoll)
        //{
        //    Student student = await _studentManager.GetStudentByClassRollAsync(stuRoll);

        //    var session = student.AcademicSession.Name.Trim().Split('-', 4);
        //    int sessionYear = Convert.ToInt32(session);

        //    DateTime startingDate = new DateTime(sessionYear,01,01);
        //    DateTime finalDate = new DateTime(sessionYear, 12, 31);

        //    return 0.0;
        //}

        private async Task<double> GetCurrentDue(int studId)
        {
            double currentDue = 0.00;
            try
            {

                Student st = await _studentManager.GetByIdAsync(studId);
                if (st == null)
                {
                    return 0;
                }

                double totalCurrentPayable = 0;
                double totalCurrentPaid = 0;
                double admissionOrSessionFee = 0;
                int feeHeadValue = 0;
                double cMonthlyFee = 0;
                double othersFee = 0;


                //0     = admission fee
                //1-12  = monthly fee
                //13    = session fee
                //14- >   other's fee

                //admission or session fee calculation
                feeHeadValue = st.AdmissionDate.Year < DateTime.Now.Year ? 13 : 0;
                admissionOrSessionFee = await _classFeeListManager.GetFeeAmountByFeeListSlAsync(st.UniqueId, feeHeadValue);

                //monthly fee calculation
                for (
                    int i = st.AdmissionDate.Month; i <= DateTime.Now.Month; i++)
                {
                    feeHeadValue = i;
                    cMonthlyFee += await _classFeeListManager.GetFeeAmountByFeeListSlAsync(st.UniqueId, feeHeadValue);
                }
                //others fee calculation
                var othersFeeList = await _classFeeListManager.GetByClassIdSessionIdStudentIdAsync(st.AcademicClassId, st.AcademicSessionId, st.Id);
                if (othersFeeList != null)
                {
                    foreach (var item in othersFeeList)
                    {
                        if (item.SL > 13)
                        {
                            othersFee += item.Amount;
                        }
                    }
                }
                totalCurrentPayable = admissionOrSessionFee + cMonthlyFee + othersFee;
                totalCurrentPaid = await GetTotalPaid(studId);
                currentDue = totalCurrentPayable - totalCurrentPaid;
            }
            catch (Exception)
            {
                throw;
            }
            return currentDue;
        }

        private async Task<double> GetFeeAsync(int aClassId, int feeHeadId, int sessionId)
        {
            ClassFeeList classFeeList = await _classFeeListManager.GetByClassIdAndFeeHeadIdAsync(aClassId, feeHeadId, sessionId);
            if (classFeeList != null)
            {
                return classFeeList.Amount;
            }
            return 0;
        }

        private async Task<double> GetTotalPaid(int stuId)
        {
            List<StudentPayment> studentPayments = (List<StudentPayment>)await _studentPaymentManager.GetAllByStudentIdAsync(stuId);
            double paidAmount = studentPayments.Sum(s => s.TotalPayment);
            return paidAmount;
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
            string receiptNo;
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

        public JsonResult GetTextByAmount(string amount)
        {
            string amountText = NumberToWords.ConvertAmount(Convert.ToDouble(amount));
            return Json("Taka " + amountText);
        }

        private List<IsActiveVM> GetActiveInActiveList()
        {
            List<IsActiveVM> isActiveVMs = new List<IsActiveVM>();

            IsActiveVM status3 = new IsActiveVM();
            status3.Id = 2;
            status3.sName = "All";
            isActiveVMs.Add(status3);

            IsActiveVM status1 = new IsActiveVM();
            status1.Id = 0;
            status1.sName = "Inactive";
            isActiveVMs.Add(status1);

            IsActiveVM status2 = new IsActiveVM();
            status2.Id = 1;
            status2.sName = "Active";
            isActiveVMs.Add(status2);
            return isActiveVMs;
        }
    }
}