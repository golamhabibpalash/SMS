using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.SetupVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using SMS.Entities.RptModels.AttendanceVM;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SetupController : Controller
    {
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        private readonly IMapper _mapper;
        public SetupController(ISetupMobileSMSManager setupMobileSMSManager, IMapper mapper)
        {
            _setupMobileSMSManager = setupMobileSMSManager;
            _mapper = mapper;
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

    }
}
