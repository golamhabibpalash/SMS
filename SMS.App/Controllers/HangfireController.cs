using BLL.Managers.Base;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.EmailServices;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq;
using SMS.DB.Migrations;
using SMS.App.ViewModels.AttendanceVM;
using System.Net;
using System.Net.Http;
using SMS.App.Utilities.ShortMessageService;
using SMS.App.Utilities.MACIPServices;
using SMS.BLL.Managers;
using static System.Net.Mime.MediaTypeNames;
using SMS.Entities.AdditionalModels;
using System.Web;

namespace SMS.App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        private readonly IStudentManager _studentManager;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IPhoneSMSManager _phoneSMSManager;
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        public HangfireController(IStudentManager studentManager, IAttendanceMachineManager attendanceMachineManager, IEmployeeManager employeeManager,IPhoneSMSManager phoneSMSManager, ISetupMobileSMSManager setupMobileSMSManager)
        {
            _studentManager = studentManager;
            _attendanceMachineManager = attendanceMachineManager;
            _employeeManager = employeeManager;
            _phoneSMSManager = phoneSMSManager;
            _setupMobileSMSManager = setupMobileSMSManager; 
        }
     

        [HttpGet]
        public IActionResult AttendanceBackgroundJob()
        {
            RecurringJob.AddOrUpdate(() => SMSSendDailyAttendanceSummary(), "0 0 10 * * SAT-THU", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => CheckInSMSSendDailyAttendanceEmployee(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);
            
            RecurringJob.AddOrUpdate(() => CheckInSMSSendDailyAttendanceStudentGirls(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);
            
            RecurringJob.AddOrUpdate(() => CheckInSMSSendDailyAttendanceStudentBoys(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);

            return Ok("Attendance Backgroud Job Started");
        }

        private void CheckInSMSSendDailyAttendanceStudentBoys()
        {
            throw new NotImplementedException();
        }

        private void CheckInSMSSendDailyAttendanceStudentGirls()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> CheckInSMSSendDailyAttendanceEmployee()
        {
            var attendanceSMSSetup = await _setupMobileSMSManager.GetByIdAsync(1);
            if (attendanceSMSSetup.CheckInSMSService == false)
            {
                return Ok("CheckIn SMS Service is Inactive");
            }
            if (attendanceSMSSetup.CheckInSMSServiceForEmployees == false)
            {
                return Ok("CheckIn SMS Service for Employees is Inactive");
            }
            var todaysAllAttendance = await _attendanceMachineManager.GetCheckinDataEmpByDate(DateTime.Now.ToString("dd-MM-yyyy"));
            if (todaysAllAttendance.Count > 0)
            {
                try
                {
                    string phoneNumber = string.Empty;
                    string smsText = string.Empty;
                    string smsType = "CheckIn";
                    string employeeName = string.Empty;
                    string attTime = string.Empty;
                    foreach (var att in todaysAllAttendance)
                    {
                        Employee empObject = await _employeeManager.GetByPhoneAttendance(att.CardNo);
                        
                        if (empObject!=null)
                        {
                            phoneNumber = empObject.Phone;

                            bool isSMSAlredySent = await _phoneSMSManager.IsSMSSendForAttendance(phoneNumber, smsType, DateTime.Now.ToString("dd-MM-yyyy"));
                            if (isSMSAlredySent)
                            {
                                continue;
                            }
                            else
                            {
                                employeeName = empObject.EmployeeName;
                                attTime = att.PunchDatetime.ToString("hh:mm");
                                smsText = GenerateCheckInSMS(employeeName,attTime);
                                bool isSend = await MobileSMS.SendSMS(phoneNumber,smsText);
                                if (isSend)
                                {
                                    PhoneSMS phoneSMS = new PhoneSMS() {
                                        Text = smsText,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = "Automation",
                                        MobileNumber = phoneNumber,
                                        MACAddress = MACService.GetMAC(),
                                        SMSType = smsType
                                    };
                                    await _phoneSMSManager.AddAsync(phoneSMS);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                
                return Ok();
            }
            else
            {
                return Ok("No Data");
            }
        }

        [HttpGet]
        public async Task SMSSendDailyAttendanceSummary()
        {
            int totalEmployee = 0;
            int totalStudent = 0;
            int totalGirlsStudent = 0;
            int totalBoysStudent = 0;

            
            try
            {
                DateTime tDate = DateTime.Today;
                var allStudentAttendance = await _attendanceMachineManager.GetAttendanceByDateAsync("student", DateTime.Today.ToString("dd-MM-yyyy"), "attended", null, null);

                var allEmployeeAttendance =await _attendanceMachineManager.GetAttendanceByDateAsync("employee", DateTime.Today.ToString("dd-MM-yyyy"), "attended", null, null);
                if (allStudentAttendance != null || allEmployeeAttendance.Count() > 0)
                {
                    var students = await _studentManager.GetAllAsync();
                    var employees = await _employeeManager.GetAllAsync();

                    totalStudent = allStudentAttendance.Count();

                    totalGirlsStudent = (from a in allStudentAttendance
                                         join s in students on a.CardNo equals s.ClassRoll.ToString()
                                         where s.GenderId == 2
                                         select a).Count();



                    totalBoysStudent = (from a in allStudentAttendance
                                        join s in students on a.CardNo equals s.ClassRoll.ToString()
                                        where s.GenderId == 1
                                        select a).Count();

                    totalEmployee = (from a in allEmployeeAttendance
                                     join e in employees on a.CardNo equals e.Phone.Substring(e.Phone.Length - 9)
                                     select a).Count();
                    string msgText = string.Empty;

                    msgText = $"Attendance Summary ({DateTime.Today.ToString("dd MMM yyyy")}):\n" +
                        $"Employees: {totalEmployee} \n" +
                        $"Students:({totalBoysStudent}+{totalGirlsStudent})= {totalStudent} \n" +                        
                        $"-Noble Residential School";

                    //Email Send
                    //string[] emails = {"golamhabibpalash@gmail.com"
                    string toEmail = "golamhabibpalash@gmail.com;sss139157@gmail.com";
                    //string toEmail = "golamhabibpalash@gmail.com";
                    string emailSubject = "Todays attended report summary";
                    string mailBody = msgText;

                    EmailService.SendEmail(toEmail, emailSubject, mailBody);
                    
                    //Phone SMS Send
                    string[] phoneNumber = { "01717678134", "01743922314" } ;
                    //string[] phoneNumber = { "01717678134" } ;
                    foreach (var num in phoneNumber)
                    {
                        bool isSend = await MobileSMS.SendSMS(num, msgText);
                        if (isSend)
                        {
                            PhoneSMS phoneSMS = new()
                            {
                                Text = msgText,
                                CreatedAt = DateTime.Now,
                                CreatedBy = "Automation",
                                MobileNumber = num,
                                MACAddress = MACService.GetMAC(),
                                SMSType = "CheckIn"
                            };
                            try
                            {
                                await _phoneSMSManager.AddAsync(phoneSMS);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }                    
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            
           
        }

        private string GenerateCheckInSMS(string name, string attendanceTime)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(attendanceTime) && !string.IsNullOrEmpty(name))
            {
                try
                {
                    msg = name + " আজ " + attendanceTime + " মিনিটে স্কুলে উপস্থিত হয়েছে। -নোবেল ।";
                    var tLength = msg.Length;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return msg;
        }
    }
}
