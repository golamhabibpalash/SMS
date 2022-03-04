using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.ShortMessageService;
using SMS.App.ViewModels.SMSVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{

    [Authorize(Roles = "SuperAdmin, Admin")]
    public class PhoneSMSController : Controller
    {
        private readonly IPhoneSMSManager _phoneSMSManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IStudentManager _studentManager;

        public PhoneSMSController(IPhoneSMSManager phoneSMSManager, IEmployeeManager employeeManager, IStudentManager studentManager)
        {
            _phoneSMSManager = phoneSMSManager;
            _employeeManager = employeeManager;
            _studentManager = studentManager;
        }
        public async Task<IActionResult> Index()
        {
            
            var allSMS = await _phoneSMSManager.GetAllAsync();
            return View(allSMS.OrderByDescending(a => a.CreatedAt));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(SMSCreateVM model)
        {
            if (ModelState.IsValid)
            {
                List<string> phoneNumbers = new List<string>();
                string[] phoneNumberArray = model.PhoneNo.Split(',');
                int sentSMSCount = 0;
                foreach (var number in phoneNumberArray)
                {
                    if (number.Length==11)
                    {
                        phoneNumbers.Add(number);
                    }
                }
                if (phoneNumbers.Count>0)
                {
                    foreach (var item in phoneNumbers)
                    {
                        bool smsSend =await MobileSMS.SendSMS(item, model.SMSText);
                        string user = HttpContext.Session.GetString("UserId");
                        
                        if (smsSend)
                        {
                            PhoneSMS phoneSMS = new() { Text = model.SMSText, CreatedAt = DateTime.Now, CreatedBy = user, MobileNumber = item };
                            await _phoneSMSManager.AddAsync(phoneSMS);
                            sentSMSCount++;
                        }
                    }
                }
                if (sentSMSCount > 0)
                {
                    TempData["smsSent"] = "Total " + sentSMSCount + " SMS has been sent.";
                }
                else
                {
                    TempData["smsNotSent"] = "Sorry, no SMS has been sent.";
                }
            }
            return RedirectToAction("Index");
        }
        
        [AllowAnonymous]
        public async Task<JsonResult> GetPhoneNumbers(string smsType, int? designationId, int? sessionId, int? classId)
        {

            List<string> phoneNumbers = new();

            if (smsType != null)
            {
                if (smsType=="e")
                {
                    var allEmployee = await _employeeManager.GetAllAsync();
                    if (designationId != null)
                    {
                        allEmployee = allEmployee.Where(e => e.DesignationId == designationId).ToList();
                    }
                    foreach (var employee in allEmployee)
                    {
                        phoneNumbers.Add(employee.Phone);
                    }
                }
                else if (smsType=="s")
                {
                    var allStudents = await _studentManager.GetAllAsync();
                    if (sessionId!=null)
                    {
                        allStudents =(from s in allStudents
                                     where s.AcademicSessionId == sessionId
                                     select s).ToList();
                        if (classId != null)
                        {
                            allStudents = allStudents.Where(s => s.AcademicClassId == classId).ToList();
                        }

                    }
                    foreach(var student in allStudents)
                    {
                        phoneNumbers.Add(student.PhoneNo);
                    }
                }
            }
            return Json(phoneNumbers);
        }


    }
}
