using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AttendancesController1cs : Controller
    {
        private readonly IAttendanceManager _attendanceManager;

        public AttendancesController1cs(IAttendanceManager attendanceManager)
        {
            _attendanceManager = attendanceManager;
        }
        // GET: AttendancesController1cs
        public ActionResult Index()
        {
            return View();
        }

        // GET: AttendancesController1cs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttendancesController1cs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendancesController1cs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: AttendancesController1cs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttendancesController1cs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: AttendancesController1cs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendancesController1cs/Delete/5
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
