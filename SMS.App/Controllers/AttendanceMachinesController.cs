using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.ViewModels.AttendanceVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public async Task<ActionResult> Index(string attendanceFor, DateTime dateTime, string attendanceType,  int? aSessionId, int? aClassId)
        {
            string date = dateTime.ToString("yyyy-MM-dd");
            
            List<AttendanceVM> attendanceVMs = new List<AttendanceVM>();

            
            ViewBag.attendanceDate = Convert.ToDateTime(dateTime).ToString("yyyy-MM-dd");
            attendanceType = String.IsNullOrEmpty(attendanceType) ? "all" : attendanceType;

            if (!string.IsNullOrEmpty(attendanceFor) && !string.IsNullOrEmpty(attendanceType))
            {
                ViewBag.attendanceFor = attendanceFor;
                ViewBag.attendanceType = attendanceType;
                ViewBag.aSessionId =aSessionId!=null? aSessionId:null;
                ViewBag.aClassId = aClassId!=null? aClassId:null;
                var result = await _attendanceMachineManager.GetAttendanceByDateAsync(attendanceFor, date, attendanceType, aSessionId, aClassId);
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.CardNo))
                    {
                        if (item.CardNo.Length <= 7)
                        {
                            Student objStudent = await _studentManager.GetStudentByClassRollAsync(Convert.ToInt32(item.CardNo));
                        }
                    }                    
                    attendanceVMs.Add(item);
                }
            }            
            return View(attendanceVMs);
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
