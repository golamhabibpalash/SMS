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
        private readonly IDesignationManager _designationManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendancesController(IAttendanceManager attendanceManager, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager, IDesignationManager designationManager)
        {
            _attendanceManager = attendanceManager;
            _studentManager = studentManager;
            _employeeManager = employeeManager;
            _designationManager = designationManager;
            _userManager = userManager;
        }
        // GET: AttendancesController
        public async Task<ActionResult> Index()
        {
            var allAttendances = await _attendanceManager.GetAllAsync();
            return View(allAttendances);
        }

        // GET: AttendancesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var att = await _attendanceManager.GetByIdAsync(id);
            if (att != null)
            {
                return View(att);
            }
            return NotFound();
        }

        // GET: AttendancesController/Create
        public ActionResult Create()
        {
            var allUser = _userManager.Users;
            ViewBag.UserList = new SelectList(allUser, "Id", "UserName");
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
            var allUser = _userManager.Users;
            ViewBag.UserList = new SelectList(allUser, "Id", "UserName",model.ApplicationUserId);
            return View();
        }

        // GET: AttendancesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var att = await _attendanceManager.GetByIdAsync(id);
            var allUser = _userManager.Users;
            ViewBag.UserList = new SelectList(allUser, "Id", "UserName", att.ApplicationUserId);
            if (att != null)
            {
                return View(att);
            }
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
                var allUser = _userManager.Users;
                ViewBag.UserList = new SelectList(allUser, "Id", "UserName", model.ApplicationUserId);
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


        public async Task<JsonResult> GetAllTodaysEmployeeByDesig(int desigId)
        {
            var designation = await _designationManager.GetByIdAsync(desigId);
            var applicationUserEmp = _userManager.Users.Where(u => u.UserType == 'e');
            var employees = await _employeeManager.GetAllAsync();
            var todaysAttendance = await _attendanceManager.GetTodaysAllAttendanceByDesigIdAsync(desigId, DateTime.Now);

            //var attendenceOfAllEmployee = from appU in applicationUserEmp
            //                              from att in todaysAttendance.Where(t => t.ApplicationUserId == appU.Id)
            //                              from e in employees
            //                              where 

            var designations = await _designationManager.GetAllAsync();
            designations = designations.Where(d => d.Employees.Count() > 0).ToList();

            var attendance = from d in designations
                             select new {designationName=d.DesignationName, attended = 1, total = d.Employees.Count() };
                             
            return Json("");
        }
    }
}
