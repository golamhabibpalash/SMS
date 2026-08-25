using Microsoft.AspNetCore.Mvc;
using SMS_App.Utilities.ShortMessageService;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace SMS_App.Controllers
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
        private readonly IAttendanceMachineService _attendanceMachineService;
        public API_AttendanceController(IStudentManager studentManager, IAttendanceMachineManager attendanceMachineManager, ISetupMobileSMSManager setupMobileSMSManager, IEmployeeManager employeeManager, IInstituteManager instituteManager, IPhoneSMSManager phoneSMSManager, IAttendanceMachineService attendanceMachineService)
        {
            _studentManager = studentManager;
            _attendanceMachineManager = attendanceMachineManager;   
            _setupMobileSMSManager = setupMobileSMSManager;
            _employeeManager = employeeManager;
            _instituteManager = instituteManager;
            _phoneSMSManager = phoneSMSManager;
            _attendanceMachineService = attendanceMachineService;
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
            TimeSpan duration =instituteInfo.ClosingTime-instituteInfo.StartingTime;
            TimeOnly middleTime =TimeOnly.FromDateTime(instituteInfo.StartingTime).Add(duration / 2);
            TimeOnly vInstituteStartingTime = TimeOnly.FromDateTime(instituteInfo.StartingTime);
            TimeOnly vInstituteClosingTime = TimeOnly.FromDateTime(instituteInfo.ClosingTime);
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
                            attendanceType = TimeOnly.FromDateTime(attendanceObject.PunchDatetime) > middleTime ? "CheckOut" : "CheckIn";
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

        // POST: api/attendance/push - Receive punch data from fingerprint machine (ZKTeco ADMS push)
        [HttpPost("push")]
        public async Task<IActionResult> ReceivePunch([FromBody] MachinePunchDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId) || dto.PunchTime == default)
            {
                return BadRequest(new { success = false, message = "Invalid payload" });
            }

            try
            {
                // Find machine by serial number
                var machines = await _attendanceMachineService.GetAllAsync();
                var machine = machines.FirstOrDefault(m => m.SerialNumber == dto.MachineSerialNo);
                
                if (machine == null)
                {
                    // Try to find by IP if serial not matched
                    var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                    machine = machines.FirstOrDefault(m => m.IPAddress == clientIp);
                }

                if (machine == null)
                {
                    return BadRequest(new { success = false, message = "Machine not registered" });
                }

                // Check if already synced (deduplication)
                var existingPunches = await _attendanceMachineManager.GetAttendanceByDateRangeAsync(
                    dto.PunchTime.AddDays(-1).ToString("yyyy-MM-dd"),
                    dto.PunchTime.ToString("yyyy-MM-dd")
                );

                var key = $"{dto.UserId}_{dto.PunchTime:yyyyMMddHHmmss}_{machine.SerialNumber ?? machine.Id.ToString()}";
                var exists = existingPunches.Any(p => $"{p.CardNo}_{p.PunchDatetime:yyyyMMddHHmmss}_{p.MachineNo}" == key);

                if (exists)
                {
                    return Ok(new { success = true, message = "Duplicate punch ignored", duplicate = true });
                }

                var punch = new Tran_MachineRawPunch
                {
                    CardNo = dto.UserId,
                    PunchDatetime = dto.PunchTime,
                    P_Day = dto.PunchTime.DayOfWeek.ToString()[0],
                    ISManual = 'N',
                    MachineNo = machine.SerialNumber ?? machine.Id.ToString(),
                    VerifyMode = dto.VerifyMode,
                    MachineSerialNo = machine.SerialNumber,
                    IsSynced = true
                };

                await _attendanceMachineManager.AddAsync(punch);

                machine.LastSyncAt = DateTime.Now;
                await _attendanceMachineService.UpdateAsync(machine);

                return Ok(new { success = true, message = "Punch recorded", punchId = punch.Tran_MachineRawPunchId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/attendance/sync-users/{machineId} - Push users to machine
        [HttpPost("sync-users/{machineId}")]
        public async Task<IActionResult> SyncUsers(int machineId)
        {
            try
            {
                var result = await _attendanceMachineService.SyncUsersToMachineAsync(machineId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/attendance/machine-status/{machineId} - Health check
        [HttpGet("machine-status/{machineId}")]
        public async Task<IActionResult> GetMachineStatus(int machineId)
        {
            try
            {
                var status = await _attendanceMachineService.GetMachineHealthAsync(machineId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/attendance/pull/{machineId} - Pull attendance from machine
        [HttpPost("pull/{machineId}")]
        public async Task<IActionResult> PullAttendance(int machineId, [FromBody] DateRangeDto dto)
        {
            try
            {
                var result = await _attendanceMachineService.PullAttendanceFromMachineAsync(machineId, dto?.FromDate, dto?.ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
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
                catch (Exception)
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
                catch (Exception)
                {

                    throw;
                }
            }
            return msg;
        }

        public class MachinePunchDto
        {
            public string MachineSerialNo { get; set; }
            public string UserId { get; set; }
            public DateTime PunchTime { get; set; }
            public int VerifyMode { get; set; }
            public int InOutMode { get; set; }
        }

        public class DateRangeDto
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }
    }
}
