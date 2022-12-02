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
using System.Collections;

namespace SMS.App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        #region Constructor Start ================================================
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
        #endregion Constructor Finished xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        [HttpGet]
        public IActionResult AttendanceBackgroundJob()
        {
            RecurringJob.AddOrUpdate(() => SMSSendDailyAttendanceSummary(), "0 0 10 * * SAT-THU", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => SendCheckInSMS(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);
            
            RecurringJob.AddOrUpdate(() => SendCheckOutSMS(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);
            
            RecurringJob.AddOrUpdate(() => CheckInSMSSendDailyAttendanceStudentBoys(), "*/20 * 8-11 * * sat-thu", TimeZoneInfo.Local);

            return Ok("Attendance Backgroud Job Started");
        }

        #region CheckIn SMS Section Start===========================================
        private async void SendCheckInSMS()
        {
            SetupMobileSMS setupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(1);
            if (setupMobileSMS!=null)
            {
                try
                {
                    if (setupMobileSMS.SMSService == true)
                    {
                        if (setupMobileSMS.CheckInSMSService == true)
                        {
                            if (setupMobileSMS.CheckInSMSServiceForEmployees == true)
                            {
                                await CheckInSMSSendDailyAttendanceEmployee();
                            }
                            if (setupMobileSMS.CheckInSMSServiceForGirlsStudent == true)
                            {
                                await CheckInSMSSendDailyAttendanceStudentGirls();
                            }
                            if (setupMobileSMS.CheckInSMSServiceForMaleStudent == true)
                            {
                                await CheckInSMSSendDailyAttendanceStudentBoys();
                            }
                        }
                        if (setupMobileSMS.CheckOutSMSService == true)
                        {
                            if (setupMobileSMS.CheckOutSMSServiceForEmployees == true)
                            {

                            }
                            if (setupMobileSMS.CheckOutSMSServiceForGirlsStudent == true)
                            {

                            }
                            if (setupMobileSMS.CheckOutSMSServiceForMaleStudent == true)
                            {

                            }
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        
        private async Task<IActionResult> CheckInSMSSendDailyAttendanceStudentBoys()
        {
            var attendanceSMSSetup = await _setupMobileSMSManager.GetByIdAsync(1);
            if (attendanceSMSSetup.CheckInSMSService == false)
            {
                return Ok("CheckIn SMS Service is Inactive");
            }
            if (attendanceSMSSetup.CheckInSMSServiceForMaleStudent == false)
            {
                return Ok("CheckIn SMS Service for Boys is Inactive");
            }
        }

        private async Task<IActionResult> CheckInSMSSendDailyAttendanceStudentGirls()
        {
            var attendanceSMSSetup = await _setupMobileSMSManager.GetByIdAsync(1);
            if (attendanceSMSSetup.CheckInSMSService == false)
            {
                return Ok("CheckIn SMS Service is Inactive");
            }
            if (attendanceSMSSetup.CheckInSMSServiceForGirlsStudent == false)
            {
                return Ok("CheckIn SMS Service for Girls is Inactive");
            }
            var todaysAllCheckInAttendance = await _attendanceMachineManager.GetAllAttendanceByDateAsync(DateTime.Today);
            var activeAllGirlStudents = await ActiveGirlsStudents();
            foreach (Tran_MachineRawPunch attendance in todaysAllCheckInAttendance)
            {
                if (attendance.CardNo.Length>7)
                {
                    continue;
                }
                foreach (Student student in activeAllGirlStudents)
                {
                    if (attendance.CardNo.Trim() == student.ClassRoll.ToString().Trim())
                    {
                        string smsText = string.Empty;
                        string smsType = "CheckIn";
                        string phoneNo = student.GuardianPhone != null? student.GuardianPhone:string.Empty;
                        string attendanceDate = DateTime.Today.ToString("dd-MM-yyyy");
                        if (!string.IsNullOrEmpty(phoneNo))
                        {
                            bool IsSentSMS = await _phoneSMSManager.IsSMSSendForAttendance(phoneNo.Trim(), smsType, attendanceDate);
                            if (IsSentSMS == false)
                            {
                                string studenName = String.IsNullOrEmpty(student.NameBangla)?student.Name:student.NameBangla;
                                smsText = GenerateCheckInSMSText(studenName, attendance.PunchDatetime.ToString());
                                bool isSent = await MobileSMS.SendSMS(phoneNo, smsText);
                                if (isSent)
                                {
                                    PhoneSMS phoneSMS = new PhoneSMS
                                    {
                                        Text = smsText,
                                        MobileNumber = phoneNo,
                                        SMSType = smsType,
                                        CreatedBy = "Automation",
                                        CreatedAt = DateTime.Now,
                                        MACAddress = MACService.GetMAC()
                                    };
                                    bool isSaved = await _phoneSMSManager.AddAsync(phoneSMS);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            return Ok();
        }


        private async Task<IActionResult> CheckInSMSSendDailyAttendanceEmployee()
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
                                smsText = GenerateCheckInSMSText(employeeName,attTime);
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
        #endregion CheckIn SMS Section Finished xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        #region CheckOut SMS Section Start ================================================
        private async Task<IActionResult> SendCheckOutSMS()
        {
            SetupMobileSMS setupMobileSMS = await _setupMobileSMSManager.GetByIdAsync(1);
            if (setupMobileSMS.CheckOutSMSService == true)
            {
                if (setupMobileSMS.CheckOutSMSServiceForEmployees == true)
                {
                    await CheckOutSMSSendDailyAttendanceEmployees();
                }
                if (setupMobileSMS.CheckOutSMSServiceForGirlsStudent == true)
                {
                    await CheckOutSMSSendDailyAttendanceGirls();
                }
                if (setupMobileSMS.CheckOutSMSServiceForMaleStudent == true)
                {
                    await CheckOutSMSSendDailyAttendanceBoys();
                }
            }
            return Ok("Send Chechout sms completed");
        }
        private async Task<IActionResult> CheckOutSMSSendDailyAttendanceBoys()
        {
            throw new NotImplementedException();
        }
        
        private async Task<IActionResult> CheckOutSMSSendDailyAttendanceGirls()
        {
            throw new NotImplementedException();
        }
        
        private async Task<IActionResult> CheckOutSMSSendDailyAttendanceEmployees()
        {
            throw new NotImplementedException();
        }
        #endregion CheckOut SMS Finished XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        #region Summary SMS Region Start Here ======================================
        private async Task SMSSendDailyAttendanceSummary()
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
        #endregion Summary SMS Region Finished Here XXXXXXXXXXXXXXXXXXXXXXX

        #region SMS Generate Section Start Here ====================================
        private string GenerateCheckInSMSText(string name, string attendanceTime)
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
        #endregion SMS Generate Section Finished Here XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


        #region User Define Methods Area==> ==> ==> ==> ==> ==> ==> ==> ==> ==> ==>
        private async Task<List<Student>> ActiveGirlsStudents()
        {
            List<Student> activeGirlStudents = new List<Student>();
            var allStudent = await _studentManager.GetAllAsync();
            if (allStudent != null)
            {
                foreach (Student student in allStudent)
                {
                    if (student.GenderId == 1 && student.Status == false)
                    {
                        continue;
                    }
                    else
                    {
                        activeGirlStudents.Add(student);
                    }
                }
            }
            return activeGirlStudents.ToList();
        }
        private async Task<List<Student>> ActiveBoysStudents()
        {
            List<Student> activeBoyStudents = new List<Student>();
            var allStudent = await _studentManager.GetAllAsync();
            if (allStudent != null)
            {
                foreach (Student student in allStudent)
                {
                    if (student.GenderId == 2 || student.Status == false)
                    {
                        continue;
                    }
                    else
                    {
                        activeBoyStudents.Add(student);
                    }
                }
            }
            return activeBoyStudents;
        }
        private async Task<List<Employee>> ActiveEmployees()
        {
            List<Employee> activeAllEmployees = new List<Employee>();
            var allEmployees = await _employeeManager.GetAllAsync();
            if (allEmployees != null)
            {
                foreach (Employee employee in allEmployees)
                {
                    if (employee.Status == true)
                    {
                        activeAllEmployees.Add(employee);
                    }
                }
            }
            return activeAllEmployees;
        }
        #endregion XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    }
}