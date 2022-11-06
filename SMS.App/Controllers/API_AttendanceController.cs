using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.ShortMessageService;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;


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
                return Ok(msg);
            }

            SetupMobileSMS smsService = await _setupMobileSMSManager.GetByIdAsync(1);
            
            if (smsService.SMSService == true)
            {
                if (smsService.AttendanceSMSService == true)
                {
                    Tran_MachineRawPunch attendanceObject = await _attendanceMachineManager.GetByIdAsync(Tran_MachineRawPunchId);
                    if (attendanceObject == null)
                    {
                        msg = "Attendance Not Found";
                        return NotFound(msg);
                    }
                    if (attendanceObject.PunchDatetime.Date != DateTime.Today.Date)
                    {
                        msg = "Your entered on " + attendanceObject.PunchDatetime.Date.ToString("dd MMM yyyy") + " but Today is " + DateTime.Today.Date.ToString("dd MMM yyyy");
                        return Ok(msg);
                    }
                    string vName = string.Empty;
                    string vAttendanceTime = string.Empty;
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
                                return Ok(msg);
                            }
                            if (TimeOnly.FromDateTime(attendanceObject.PunchDatetime) < vSMSStartingTime || TimeOnly.FromDateTime(attendanceObject.PunchDatetime) > vSMSClosingTime)
                            {
                                msg = "Information time is not valid";
                                return Ok(msg);
                            }

                            vAttendanceTime = TimeOnly.FromDateTime(attendanceObject.PunchDatetime).ToString();
                            attendanceType = attendanceObject.PunchDatetime.TimeOfDay > middleTime.TimeOfDay ? "CheckOut" : "CheckIn";
                            phoneSMSObject.SMSType = attendanceType;
                            attendanceFor = attendanceObject.CardNo.Length > 7 ? "employee" : "student";
                            if (attendanceFor == "student")
                            {
                                Student studentObject = await _studentManager.GetStudentByClassRollAsync(int.Parse(attendanceObject.CardNo));
                                if (studentObject != null)
                                {
                                    vName = string.IsNullOrEmpty(studentObject.NameBangla) ? studentObject.Name : studentObject.NameBangla;
                                    //For boys student
                                    if (studentObject.GenderId == 1)
                                    {
                                        if (attendanceType == "CheckIn")
                                        {
                                            if (smsService.CheckInSMSServiceForMaleStudent == true)
                                            {
                                                phoneSMSObject.Text = GenerateCheckInSMS(vName, vAttendanceTime);
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
                                                phoneSMSObject.Text = GenerateCheckOutSMS(vName, vAttendanceTime);
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
                                                phoneSMSObject.Text = GenerateCheckInSMS(vName, vAttendanceTime);
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
                                                phoneSMSObject.Text = GenerateCheckOutSMS(vName, vAttendanceTime);
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
                                    return Ok(msg);
                                }
                            }
                            else //For Employees 
                            {
                                Employee employee = await _employeeManager.GetByPhoneAttendance(attendanceObject.CardNo);

                                if (employee != null)
                                {
                                    vName = string.IsNullOrEmpty(employee.EmployeeNameBangla) ? employee.EmployeeName : employee.EmployeeNameBangla;
                                    if (attendanceType == "CheckIn")
                                    {
                                        if (smsService.CheckInSMSServiceForEmployees == true)
                                        {
                                            phoneSMSObject.Text = GenerateCheckInSMS(vName, vAttendanceTime);
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
                                            phoneSMSObject.Text = GenerateCheckOutSMS(vName,vAttendanceTime);
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
                                    return Ok(msg);
                                }
                            }
                        }
                        else
                        {
                            msg = "Attendance not found";
                            return Ok(msg);
                        }
                            if (!string.IsNullOrEmpty(phoneSMSObject.Text))
                            {
                            int tLength = phoneSMSObject.Text.Length;

                            bool isSMSAlreadySent = await _phoneSMSManager.IsSMSSendForAttendance(phoneSMSObject.MobileNumber, phoneSMSObject.SMSType,phoneSMSObject.CreatedAt.ToString("yyyyMMdd"));
                            if (isSMSAlreadySent)
                            {
                                msg = phoneSMSObject.SMSType + " SMS already sent for this user";
                                return Ok(msg);
                            }

                            bool isSend = await MobileSMS.SendSMS(phoneSMSObject.MobileNumber,phoneSMSObject.Text);
                                if (isSend)
                                {
                                    bool isSaved = await _phoneSMSManager.AddAsync(phoneSMSObject);
                                    if (isSaved)
                                    {
                                        msg = "SMS Save and Send Successfuly";
                                    }
                                }
                                else
                                {
                                msg = "Service balance finished or SMS Service provider problem.";
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
                    msg =name+" আজ "+attendanceTime+" মিনিটে স্কুলে উপস্থিত হয়েছে। -নোবেল ।";
                    var tLength = msg.Length;
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
                    msg = name + " স্কুল থেকে " + attendanceTime + " মিনিটে প্রস্থান করেছে। -নোবেল ।";
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
