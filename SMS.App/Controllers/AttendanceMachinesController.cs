using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.ViewModels.AttendanceVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AttendanceMachinesController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IStudentManager _studentManager;
        private readonly IDesignationManager _designationManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAcademicSessionManager _academicSessionManager;

        public AttendanceMachinesController(IHttpContextAccessor contextAccessor, IAttendanceMachineManager attendanceMachineManager, IEmployeeManager employeeManager, IStudentManager studentManager, IDesignationManager designationManager, IAcademicClassManager academicClassManager, IAcademicSessionManager academicSessionManager)
        {
            _contextAccessor = contextAccessor;
            _attendanceMachineManager = attendanceMachineManager;
            _employeeManager = employeeManager;
            _studentManager = studentManager;
            _designationManager = designationManager;
            _academicClassManager = academicClassManager;
            _academicSessionManager = academicSessionManager;
        }
        // GET: AttendanceMachinesController
        public async Task<ActionResult> Index(DateTime? dateTime, string userType,int designationId, int sessionId, int classId, string attendanceType)
        {
            var allAttendance = await _attendanceMachineManager.GetAllAsync();
            List<AttendanceMachineIndexVM> attendanceMachineIndexVMs = new List<AttendanceMachineIndexVM>();
            foreach (var item in allAttendance)
            {
                AttendanceMachineIndexVM attendanceMachineIndexVM = new AttendanceMachineIndexVM();
                attendanceMachineIndexVM.Id = item.Tran_MachineRawPunchId;
                attendanceMachineIndexVM.CardNo = item.CardNo;
                attendanceMachineIndexVM.PunchDateTime = item.PunchDatetime;
                attendanceMachineIndexVM.MachineNo = item.MachineNo;
                if (item.CardNo.Length>7)
                {
                    Employee emp = await _employeeManager.GetByPhoneAttendance(item.CardNo);
                    if (emp != null)
                    {
                        attendanceMachineIndexVM.Name = emp.EmployeeName;
                        Designation designation = await _designationManager.GetByIdAsync(emp.DesignationId);
                        attendanceMachineIndexVM.UserInfo = "Employee (" + designation.DesignationName + ")";
                        attendanceMachineIndexVM.UserType = "e";
                        attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
                    }
                }
                else
                {
                    Student student = await _studentManager.GetStudentByClassRollAsync(Convert.ToInt32(item.CardNo));
                    if (student != null)
                    {
                        attendanceMachineIndexVM.Name = student.Name;
                        AcademicClass academicClass = await _academicClassManager.GetByIdAsync(student.AcademicClassId);
                        attendanceMachineIndexVM.UserInfo = "Student (" + academicClass.Name + ")";
                        attendanceMachineIndexVM.UserType = "s";
                        attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
                    }
                }
            }
            

                                        
            if (attendanceType != null)
            {
                if (attendanceType.ToLower() == "attended")
                {
                    if (userType != null)
                    {
                        attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.UserType.ToLower() == userType.ToLower()).ToList();
                        if (userType == "e")
                        {
                            if (designationId > 0)
                            {
                                var allEmployee = await _employeeManager.GetAllAsync();
                                attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                             from e in allEmployee.Where(r => r.Phone.Substring(2, r.Phone.Length) == a.CardNo)
                                                             where e.DesignationId == designationId
                                                             select a).ToList();
                            }
                        }
                        else if (userType == "s")
                        {
                            if (classId > 0)
                            {
                                var allStudent = await _studentManager.GetAllAsync();
                                attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                             from s in allStudent.Where(t => t.ClassRoll.ToString() == a.CardNo)
                                                             where s.AcademicClassId == classId
                                                             select a).ToList();
                                if (sessionId > 0)
                                {
                                    var allSession = await _academicSessionManager.GetAllAsync();
                                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                                 from s in allStudent.Where(t => t.ClassRoll.ToString() == a.CardNo)
                                                                 where s.AcademicSessionId == sessionId
                                                                 select a).ToList();
                                }
                            }
                        }
                    }
                }
                else if (attendanceType == "absent")
                {
                    if (userType != null)
                    {
                        attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.UserType.ToLower() == userType.ToLower()).ToList();
                        if (userType == "e")
                        {
                            if (designationId > 0)
                            {
                                var allEmployee = await _employeeManager.GetAllAsync();
                                attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                             from e in allEmployee.Where(r => r.Phone.Substring(2, r.Phone.Length) != a.CardNo)
                                                             where e.DesignationId == designationId
                                                             select a).ToList();
                            }
                        }
                        else if (userType == "s")
                        {
                            if (classId > 0)
                            {
                                var allStudent = await _studentManager.GetAllAsync();
                                attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                             from s in allStudent.Where(t => t.ClassRoll.ToString() != a.CardNo)
                                                             where s.AcademicClassId == classId
                                                             select a).ToList();
                                if (sessionId > 0)
                                {
                                    var allSession = await _academicSessionManager.GetAllAsync();
                                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
                                                                 from s in allStudent.Where(t => t.ClassRoll.ToString() != a.CardNo)
                                                                 where s.AcademicSessionId == sessionId
                                                                 select a).ToList();
                                }
                            }
                        }
                    }
                }
            }
            if (dateTime != null)
            {
                attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.PunchDateTime.Date == Convert.ToDateTime(dateTime).Date).ToList();
            }
            attendanceMachineIndexVMs = attendanceMachineIndexVMs.GroupBy(x => x.CardNo).Select(y => y.FirstOrDefault()).ToList();
            return View(attendanceMachineIndexVMs);
        }


        public async Task<ActionResult> Details(int id)
        {
            var attendance = await _attendanceMachineManager.GetByIdAsync(id);
            return View(attendance);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Tran_MachineRawPunch model)
        {
            if (ModelState.IsValid)
            {
                await _attendanceMachineManager.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: AttendanceMachinesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var attendance = await _attendanceMachineManager.GetByIdAsync(id);
            return View(attendance);
        }

        // POST: AttendanceMachinesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Tran_MachineRawPunch model)
        {
            if (id != model.Tran_MachineRawPunchId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _attendanceMachineManager.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: AttendanceMachinesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendanceMachinesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
