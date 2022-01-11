using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly IAttendanceManager _attendanceManager;
        private readonly IStudentManager _studentManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendancesController(IAttendanceManager attendanceManager, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager)
        {
            _attendanceManager = attendanceManager;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _userManager = userManager;
        }
        // GET: AttendancesController
        public async Task<ActionResult> Index()
        {
            var allAttendances = await _attendanceManager.GetAllAsync();
            return View(allAttendances);
        }

        // GET: AttendancesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttendancesController/Create
        public ActionResult Create()
        {
            var allUser = _userManager.Users;
            ViewBag.ApplicationUserId = new SelectList(allUser, "Id", "UserName");
            return View();
        }

        // POST: AttendancesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Attendance model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("UserId");

                    await _attendanceManager.AddAsync(model);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                    throw;
                }
                
            }
            return View();
        }

        // GET: AttendancesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttendancesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Attendance model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            try
            {
                model.EditedAt = DateTime.Now;
                model.EditedBy = HttpContext.Session.GetString("UserId");
                bool isUpdated = await _attendanceManager.UpdateAsync(model);
                if (isUpdated)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendancesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Attendance existingAttendances = await _attendanceManager.GetByIdAsync(id);
            if (existingAttendances != null)
            {
                return View(existingAttendances);
            }
            ViewBag.msg = "Attendance not found";
            return View();
        }

        // POST: AttendancesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                if (id != attendance.Id)
                {
                    return NotFound();
                }
                await _attendanceManager.RemoveAsync(attendance);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<JsonResult> GetAllTodaysEmployeeByDegis(int desigId)
        {
            var attendedEmployee = await _attendanceManager.GetTodaysAllAttendanceByDesigIdAsync(desigId, DateTime.Now);
            return Json(attendedEmployee);
        }
    }
}
