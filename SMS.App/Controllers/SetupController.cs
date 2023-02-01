﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.App.ViewModels.SetupVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Data;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SetupController : Controller
    {
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        public SetupController(ISetupMobileSMSManager setupMobileSMSManager)
        {
            _setupMobileSMSManager = setupMobileSMSManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SMSControl()
        {
            SetupMobileSMS setupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(1);
            AttendanceSetupVM attendanceSetupVM = new AttendanceSetupVM();

            attendanceSetupVM.Id = setupMobileSMS.Id;

            attendanceSetupVM.AttendanceSMSService = setupMobileSMS.AttendanceSMSService;

            attendanceSetupVM.CheckInSMS = setupMobileSMS.CheckInSMSService;            
            attendanceSetupVM.CheckInSMSEmployees = setupMobileSMS.CheckInSMSServiceForEmployees;
            attendanceSetupVM.CheckInSMSStudentBoys = setupMobileSMS.CheckInSMSServiceForMaleStudent;
            attendanceSetupVM.CheckInSMSStudentGirls = setupMobileSMS.CheckInSMSServiceForGirlsStudent;

            attendanceSetupVM.CheckOutSMS = setupMobileSMS.CheckOutSMSService;
            attendanceSetupVM.CheckOutSMSEmployees = setupMobileSMS.CheckOutSMSServiceForEmployees;
            attendanceSetupVM.CheckOutSMSStudentBoys = setupMobileSMS.CheckOutSMSServiceForMaleStudent;
            attendanceSetupVM.CheckOutSMSStudentGirls = setupMobileSMS.CheckOutSMSServiceForGirlsStudent;

            attendanceSetupVM.CheckInSMSSummary = setupMobileSMS.CheckInSMSSummary;
            attendanceSetupVM.AbsentNotification = setupMobileSMS.AbsentNotification;
            //if (setupMobileSMS != null)
            //{
            //    return View(setupMobileSMS);
            //}
            return View(attendanceSetupVM);
        }

        [HttpPost]
        public async Task<IActionResult> SMSControl(AttendanceSetupVM attendanceSetupVM)
        {
            string msg = "";
            if (attendanceSetupVM!=null)
            {
                SetupMobileSMS objSetupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(attendanceSetupVM.Id);

                objSetupMobileSMS.AttendanceSMSService = attendanceSetupVM.AttendanceSMSService;

                objSetupMobileSMS.CheckInSMSService = attendanceSetupVM.CheckInSMS;
                objSetupMobileSMS.CheckInSMSServiceForEmployees = attendanceSetupVM.CheckInSMSEmployees;
                objSetupMobileSMS.CheckInSMSServiceForMaleStudent = attendanceSetupVM.CheckInSMSStudentBoys;
                objSetupMobileSMS.CheckInSMSServiceForGirlsStudent = attendanceSetupVM.CheckInSMSStudentGirls;


                objSetupMobileSMS.CheckOutSMSService = attendanceSetupVM.CheckOutSMS;
                objSetupMobileSMS.CheckOutSMSServiceForEmployees = attendanceSetupVM.CheckOutSMSEmployees;
                objSetupMobileSMS.CheckOutSMSServiceForMaleStudent = attendanceSetupVM.CheckOutSMSStudentBoys;
                objSetupMobileSMS.CheckOutSMSServiceForGirlsStudent = attendanceSetupVM.CheckOutSMSStudentGirls;
                objSetupMobileSMS.AbsentNotification = attendanceSetupVM.AbsentNotification;
                objSetupMobileSMS.CheckInSMSSummary = attendanceSetupVM.CheckInSMSSummary;

                try
                {
                    bool isUpdated = await _setupMobileSMSManager.UpdateAsync(objSetupMobileSMS);
                    if (isUpdated)
                    {
                        msg = "SMS Setup Updated successful";
                        TempData["edited"] = msg;
                        return RedirectToAction("AttendanceBackgroundJob", "Hangfire");
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
    }
}