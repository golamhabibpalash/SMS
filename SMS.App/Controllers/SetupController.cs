using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.SetupVM;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.Entities;
using SMS.Entities.RptModels.AttendanceVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SetupController : Controller
    {
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        private readonly IMapper _mapper;
        private readonly IStudentManager _studentManager;  
        private readonly IAcademicClassManager _academicClassManager;
        public SetupController(ISetupMobileSMSManager setupMobileSMSManager, IMapper mapper, IStudentManager studentManager, IAcademicClassManager academicClassManager)
        {
            _setupMobileSMSManager = setupMobileSMSManager;
            _mapper = mapper;
            _studentManager = studentManager;
            _academicClassManager = academicClassManager;
        }

        [Authorize(Policy = "IndexSetupPolicy")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "SMSControlSetupPolicy")]
        public async Task<IActionResult> SMSControl()
        {
            SetupMobileSMS setupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(1);
            AttendanceSetupVM attendanceSetupVM = new AttendanceSetupVM();
            try
            {
                attendanceSetupVM = _mapper.Map<AttendanceSetupVM>(setupMobileSMS);
            }
            catch (System.Exception)
            {
                throw;
            }
            
            return View(attendanceSetupVM);
        }

        [HttpPost]
        [Authorize(Policy = "SMSControlSetupPolicy")]
        public async Task<IActionResult> SMSControl(AttendanceSetupVM attendanceSetupVM)
        {
            string msg = "";
            if (attendanceSetupVM!=null)
            {
                SetupMobileSMS objSetupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(attendanceSetupVM.Id);

                objSetupMobileSMS = _mapper.Map<SetupMobileSMS>(attendanceSetupVM);

                objSetupMobileSMS.EditedAt = DateTime.Now;
                objSetupMobileSMS.EditedBy = HttpContext.Session.GetString("UserId");
                objSetupMobileSMS.MACAddress = MACService.GetMAC();
                try
                {
                    bool isUpdated = await _setupMobileSMSManager.UpdateAsync(objSetupMobileSMS);
                    if (isUpdated)
                    {
                        msg = "SMS Setup Updated successful";
                        TempData["edited"] = msg;
                        return RedirectToAction("AttendanceBackgroundJob", "Hangfire");
                        //TempData["edited"] = msg;
                        //attendanceSetupVM.EditedAt = DateTime.Now;
                        //attendanceSetupVM.EditedBy = HttpContext.Session.GetString("UserId");   
                        //return View(attendanceSetupVM);
                    }
                    else
                    {
                        msg = "Fail to save";
                    }
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
            return View(attendanceSetupVM);
        }

        [HttpGet]
        [Authorize(Policy = "StudentWiseSMSServiceSetupPolicy")]
        public async Task<IActionResult> StudentWiseSMSService(int? academicClassId)
        {
            var students = await _studentManager.GetAllAsync();
            students = students.Where(s => s.Status == true).ToList();
            StudentWiseSMSServiceVM studentWiseSMSServiceVM = new StudentWiseSMSServiceVM();
            if (academicClassId!=null)
            {
                students = students.Where(s => s.AcademicClassId == academicClassId).ToList();
                studentWiseSMSServiceVM.Students = (List<Student>)students;
            }

            ViewBag.academicClassId = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicClassId);

            return View(studentWiseSMSServiceVM); 
        }

        [HttpPost]
        [Authorize(Policy = "StudentWiseSMSServiceSetupPolicy")]
        public async Task<IActionResult> StudentWiseSMSService(StudentWiseSMSServiceVM studentWiseSMSServiceVM)
        {
            if (studentWiseSMSServiceVM!=null)
            {
                try
                {
                    int totalUpdated = 0;
                    foreach (var student in studentWiseSMSServiceVM.Students)
                    {
                        Student student1 = await _studentManager.GetByIdAsync(student.Id);
                        if (student1.SMSService != student.SMSService)
                        {
                            student1.SMSService = student.SMSService;
                            await _studentManager.UpdateAsync(student1);
                            totalUpdated++; 
                        }
                    }
                    ViewBag.totalUpdated = totalUpdated;
                    TempData["updated"] = "Total " + totalUpdated + " data updated";
                }
                catch (Exception)
                {
                    throw;
                }
            }
            ViewBag.academicClassId = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            return View();
        }
    }
}
