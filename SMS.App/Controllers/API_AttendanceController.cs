using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.ShortMessageService;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SMS.App.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class API_AttendanceController : ControllerBase
    {
        private readonly IStudentManager _studentManager;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly ISetupMobileSMSManager _setupMobileSMSManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IInstituteManager _instituteManager;
        private readonly IPhoneSMSManager _phoneSMSManager;
        public API_AttendanceController(IStudentManager studentManager, IAttendanceMachineManager attendanceMachineManager,ISetupMobileSMSManager setupMobileSMSManager, IEmployeeManager employeeManager, IInstituteManager instituteManager, IPhoneSMSManager phoneSMSManager)
        {
            _studentManager = studentManager;
            _attendanceMachineManager = attendanceMachineManager;   
            _setupMobileSMSManager = setupMobileSMSManager;
            _employeeManager = employeeManager;
            _instituteManager = instituteManager;
            _phoneSMSManager = phoneSMSManager;
        }


        // GET: api/<API_AttendanceController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        public async Task<IActionResult> SendSMS(int Tran_MachineRawPunchId)
        {
            string msg = string.Empty;
            string attendanceType = string.Empty;//CheckIn or CheckOut
            string attendanceFor = string.Empty;//student, employee
            
            Institute instituteInfo = await _instituteManager.GetByIdAsync(1);
            TimeSpan duration = DateTime.Parse(instituteInfo.ClosingTime).Subtract(DateTime.Parse(instituteInfo.StartingTime));
            DateTime middleTime = DateTime.Parse(instituteInfo.StartingTime).Add(duration / 2);
            TimeOnly vInstituteStartingTime = TimeOnly.FromDateTime(DateTime.Parse(instituteInfo.StartingTime));
            TimeOnly vInstituteClosingTime = TimeOnly.FromDateTime(DateTime.Parse(instituteInfo.ClosingTime));
            TimeOnly vSMSStartingTime = vInstituteStartingTime.AddHours(-1.00);
            TimeOnly vSMSClosingTime = vInstituteClosingTime.AddHours(1.00);
            if (TimeOnly.FromDateTime(DateTime.Now)<vSMSStartingTime || TimeOnly.FromDateTime(DateTime.Now) > vSMSClosingTime)
            {
                msg = "SMS sending time from " + vSMSStartingTime.ToString() + " to " + vSMSClosingTime.ToString();
                return NotFound(msg);
            }
            SetupMobileSMS smsService = await _setupMobileSMSManager.GetByIdAsync(1);
            
            if (smsService.SMSService == true)
            {
                if (smsService.AttendanceSMSService == true)
                {
                    Tran_MachineRawPunch attendanceObject = await _attendanceMachineManager.GetByIdAsync(Tran_MachineRawPunchId);
                    try
                    {
                        PhoneSMS phoneSMSObject = new PhoneSMS() {
                            MACAddress = "System Generate",
                            CreatedAt = DateTime.Now,
                            CreatedBy = "Punch Machine"
                        };
                        
                        if (attendanceObject != null)
                        {
                            if (attendanceObject.PunchDatetime.ToString("yyyyMMdd") != DateTime.Today.ToString("yyyyMMdd"))
                            {
                                msg = "Information date is not valid";
                                return BadRequest(msg);
                            }
                            if (TimeOnly.FromDateTime(attendanceObject.PunchDatetime) < vSMSStartingTime || TimeOnly.FromDateTime(attendanceObject.PunchDatetime) > vSMSClosingTime)
                            {
                                msg = "Information time is not valid";
                                return BadRequest(msg);
                            }
                            attendanceType = attendanceObject.PunchDatetime.TimeOfDay > middleTime.TimeOfDay ? "CheckOut" : "CheckIn";
                            attendanceFor = attendanceObject.CardNo.Length > 7 ? "employee" : "student";
                            if (attendanceFor == "student")
                            {
                                Student studentObject = await _studentManager.GetStudentByClassRollAsync(int.Parse(attendanceObject.CardNo));
                                if (studentObject != null)
                                {
                                    //For boys student
                                    if (studentObject.GenderId == 1)
                                    {
                                        if (attendanceType == "CheckIn")
                                        {
                                            if (smsService.CheckInSMSServiceForMaleStudent == true)
                                            {
                                                phoneSMSObject.Text = GenerateCheckInSMS(studentObject.Name, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                                phoneSMSObject.MobileNumber = studentObject.GuardianPhone;
                                            }
                                            else
                                            {
                                                msg = "Boys student checkin SMS Service is turned off";
                                            }
                                        }
                                        else
                                        {
                                            if (smsService.CheckOutSMSServiceForMaleStudent == true)
                                            {
                                                phoneSMSObject.Text = GenerateCheckOutSMS(studentObject.Name, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                                phoneSMSObject.MobileNumber = studentObject.GuardianPhone;
                                            }
                                            else
                                            {
                                                msg = "Boys student checkout SMS Service is turned off";
                                            }
                                        }
                                    }
                                    else //For Girls Students
                                    {
                                        if (attendanceType == "CheckIn")
                                        {
                                            if (smsService.CheckInSMSServiceForGirlsStudent == true)
                                            {
                                                phoneSMSObject.Text = GenerateCheckInSMS(studentObject.Name, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                                phoneSMSObject.MobileNumber = studentObject.GuardianPhone;
                                            }
                                            else
                                            {
                                                msg = "Girls student checkin SMS Service is turned off";
                                            }
                                        }
                                        else
                                        {
                                            if (smsService.CheckOutSMSServiceForGirlsStudent == true)
                                            {
                                                phoneSMSObject.Text = GenerateCheckOutSMS(studentObject.Name, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                                phoneSMSObject.MobileNumber = studentObject.GuardianPhone;
                                            }
                                            else
                                            {
                                                msg = "Girls student checkout SMS Service is turned off";
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    msg = "Student Not Found";
                                    return NotFound(msg);
                                }
                            }
                            else //For Employees 
                            {
                                Employee employee = await _employeeManager.GetByPhoneAttendance(attendanceObject.CardNo);
                                if (employee != null)
                                {
                                    if (attendanceType == "CheckIn")
                                    {
                                        if (smsService.CheckInSMSServiceForEmployees == true)
                                        {
                                            phoneSMSObject.Text = GenerateCheckInSMS(employee.EmployeeName, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                            phoneSMSObject.MobileNumber = employee.Phone;
                                        }
                                        else
                                        {
                                            msg = "Employees checkin SMS Service is turned off";
                                        }
                                    }
                                    else
                                    {
                                        if (smsService.CheckOutSMSServiceForEmployees == true)
                                        {
                                            phoneSMSObject.Text = GenerateCheckOutSMS(employee.EmployeeName, attendanceObject.PunchDatetime.ToString("hh:mm tt"));
                                            phoneSMSObject.MobileNumber = employee.Phone;
                                        }
                                        else
                                        {
                                            msg = "Employees checkout SMS Service is turned off";
                                        }
                                    }
                                }
                                else
                                {
                                    msg = "Employee Not Found";
                                    return NotFound(msg);
                                }
                            }
                        }
                        else
                        {
                            msg = "Attendance not found";
                            return NotFound(msg);
                        }
                        if (!string.IsNullOrEmpty(phoneSMSObject.Text))
                        {
                            bool isSend = await MobileSMS.SendSMS(phoneSMSObject.Text, phoneSMSObject.MobileNumber);
                            if (isSend)
                            {
                                bool isSaved = await _phoneSMSManager.AddAsync(phoneSMSObject);
                                if (isSaved)
                                {
                                    msg = "SMS Save and Send Succeffulyy";
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    
                }
                else
                {
                    msg = "Attendance SMS Service is turned off";
                }
            }
            else
            {
                msg = "SMS Service is turned off";
            }

            return Ok(msg);
        }
        // GET api/<API_AttendanceController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<API_AttendanceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<API_AttendanceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<API_AttendanceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string GenerateCheckInSMS(string name, string attendanceTime)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(attendanceTime) && !string.IsNullOrEmpty(name))
            {
                try
                {
                    msg =name+" সকাল "+attendanceTime+" মিনিটে স্কুলে উপস্থিত হয়েছে। -নোবেল।";
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return msg;
        }
        private string GenerateCheckOutSMS(string name, string attendanceTime)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(attendanceTime) && !string.IsNullOrEmpty(name))
            {
                try
                {
                    msg = name + " স্কুল থেকে " + attendanceTime + " মিনিটে প্রস্থান করেছে। -নোবেল।";
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return msg;
        }
    }
}
