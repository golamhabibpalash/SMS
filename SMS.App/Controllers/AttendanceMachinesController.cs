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
        public async Task<ActionResult> Index(DateTime? dateTime, string userType, int designationId, int sessionId, int classId, string attendanceType)
        {

            List<AttendanceMachineIndexVM> attendanceMachineIndexVMs = new List<AttendanceMachineIndexVM>();

            if (dateTime == null)
            {
                return View(attendanceMachineIndexVMs);
            }
            ViewBag.attendanceDate = Convert.ToDateTime(dateTime).ToString("dd MMM yyyy");
            var allAttendanceByDate = await _attendanceMachineManager.GetAllAttendanceByDateAsync((DateTime)dateTime);
            foreach (var attended in allAttendanceByDate)
            {
                AttendanceMachineIndexVM attendanceMachineIndexVM = new AttendanceMachineIndexVM();
                attendanceMachineIndexVM.Id = attended.Tran_MachineRawPunchId;
                attendanceMachineIndexVM.CardNo = attended.CardNo;
                attendanceMachineIndexVM.MachineNo = attended.MachineNo;
                attendanceMachineIndexVM.PunchDateTime = attended.PunchDatetime;
                if (userType == "e")
                {
                    if (attended.CardNo.Length > 7)
                    {
                        Employee emp = await _employeeManager.GetByPhoneAttendance(attended.CardNo);
                        if (emp != null)
                        {
                            attendanceMachineIndexVM.Name = emp.EmployeeName;
                            Designation designation = await _designationManager.GetByIdAsync(emp.DesignationId);
                            attendanceMachineIndexVM.UserInfo = "Employee (" + designation.DesignationName + ")";
                            attendanceMachineIndexVM.Designation = designation.DesignationName;
                            attendanceMachineIndexVM.Phone = emp.Phone;
                            attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
                        }
                    }
                    ViewBag.attendanceFor = "Employees";
                }
                else if (userType == "s")
                {
                    Student student = await _studentManager.GetStudentByClassRollAsync(Convert.ToInt32(attended.CardNo)); if (student != null)
                    {
                        attendanceMachineIndexVM.Name = student.Name;
                        attendanceMachineIndexVM.GuardianPhone = student.GuardianPhone;
                        attendanceMachineIndexVM.Phone = student.PhoneNo;
                        AcademicClass academicClass = await _academicClassManager.GetByIdAsync(student.AcademicClassId);
                        attendanceMachineIndexVM.UserInfo = "Student (" + academicClass.Name + ")";
                        if (classId > 0)
                        {
                            if (academicClass.Id == classId)
                            {
                                attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
                            }
                        }
                        else
                        {
                            attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
                        }
                    }
                    ViewBag.attendanceFor = "Students";
                }

                
            }
            return View(attendanceMachineIndexVMs);

            #region attendanceType Code
            //if (attendanceType != null)
            //{
            //    if (attendanceType.ToLower() == "attended")
            //    {
            //        if (userType != null)
            //        {
            //            attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.UserType.ToLower() == userType.ToLower()).ToList();
            //            if (userType == "e")
            //            {
            //                if (designationId > 0)
            //                {
            //                    var allEmployee = await _employeeManager.GetAllAsync();
            //                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                 from e in allEmployee.Where(r => r.Phone.Substring(r.Phone.Length - 9) == a.CardNo)
            //                                                 where e.DesignationId == designationId
            //                                                 select a).ToList();
            //                }

            //            }
            //            else if (userType == "s")
            //            {
            //                if (classId > 0)
            //                {
            //                    var allStudent = await _studentManager.GetAllAsync();
            //                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                 from s in allStudent.Where(t => t.ClassRoll.ToString() == a.CardNo)
            //                                                 where s.AcademicClassId == classId
            //                                                 select a).ToList();
            //                    if (sessionId > 0)
            //                    {
            //                        var allSession = await _academicSessionManager.GetAllAsync();
            //                        attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                     from s in allStudent.Where(t => t.ClassRoll.ToString() == a.CardNo)
            //                                                     where s.AcademicSessionId == sessionId
            //                                                     select a).ToList();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else if (attendanceType == "absent")
            //    {
            //        if (userType != null)
            //        {
            //            attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.UserType.ToLower() == userType.ToLower()).ToList();
            //            if (userType == "e")
            //            {
            //                if (designationId > 0)
            //                {
            //                    var allEmployee = await _employeeManager.GetAllAsync();
            //                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                 from e in allEmployee.Where(r => r.Phone.Substring(2, r.Phone.Length) != a.CardNo)
            //                                                 where e.DesignationId == designationId
            //                                                 select a).ToList();
            //                }
            //            }
            //            else if (userType == "s")
            //            {
            //                if (classId > 0)
            //                {
            //                    var allStudent = await _studentManager.GetAllAsync();
            //                    attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                 from s in allStudent.Where(t => t.ClassRoll.ToString() != a.CardNo)
            //                                                 where s.AcademicClassId == classId
            //                                                 select a).ToList();
            //                    if (sessionId > 0)
            //                    {
            //                        var allSession = await _academicSessionManager.GetAllAsync();
            //                        attendanceMachineIndexVMs = (from a in attendanceMachineIndexVMs
            //                                                     from s in allStudent.Where(t => t.ClassRoll.ToString() != a.CardNo)
            //                                                     where s.AcademicSessionId == sessionId
            //                                                     select a).ToList();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //if (dateTime != null)
            //{
            //    attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.PunchDateTime.Date == Convert.ToDateTime(dateTime).Date).ToList();
            //}
            //attendanceMachineIndexVMs = attendanceMachineIndexVMs.GroupBy(x => x.CardNo).Select(y => y.FirstOrDefault()).ToList();
            #endregion


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
