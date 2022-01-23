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

        public AttendanceMachinesController(IHttpContextAccessor contextAccessor, IAttendanceMachineManager attendanceMachineManager, IEmployeeManager employeeManager, IStudentManager studentManager, IDesignationManager designationManager, IAcademicClassManager academicClassManager)
        {
            _contextAccessor = contextAccessor;
            _attendanceMachineManager = attendanceMachineManager;
            _employeeManager = employeeManager;
            _studentManager = studentManager;
            _designationManager = designationManager;
            _academicClassManager = academicClassManager;
        }
        // GET: AttendanceMachinesController
        public async Task<ActionResult> Index(DateTime? dateTime)
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
                        attendanceMachineIndexVM.UserType = "Employee ("+designation.DesignationName+")";
                    }
                }
                else
                {
                    Student student = await _studentManager.GetStudentByClassRollAsync(Convert.ToInt32(item.CardNo));
                    if (student != null)
                    {
                        attendanceMachineIndexVM.Name = student.Name;
                        AcademicClass academicClass = await _academicClassManager.GetByIdAsync(student.AcademicClassId);
                        attendanceMachineIndexVM.UserType = "Student ("+academicClass.Name+")";
                    }
                }
                attendanceMachineIndexVMs.Add(attendanceMachineIndexVM);
            }
            if (dateTime != null)
            {
                attendanceMachineIndexVMs = attendanceMachineIndexVMs.Where(a => a.PunchDateTime.Date == Convert.ToDateTime(dateTime).Date).ToList();
            }
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
