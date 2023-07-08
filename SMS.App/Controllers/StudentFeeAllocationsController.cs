using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.ViewModels.Students;
using SMS.BLL.Contracts;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using System;
using SMS.App.Utilities.MACIPServices;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class StudentFeeAllocationsController : Controller
    {
        private readonly IStudentFeeAllocationManager _studentFeeAllocationManager;
        private readonly IStudentFeeHeadManager _studentFeeHeadManager;
        private readonly IAcademicClassManager _academicClassManager;
        public StudentFeeAllocationsController(IStudentFeeAllocationManager studentFeeAllocationManager, IStudentFeeHeadManager studentFeeHeadManager, IAcademicClassManager academicClassManager)
        {
            _studentFeeAllocationManager = studentFeeAllocationManager;
            _studentFeeHeadManager = studentFeeHeadManager;
            _academicClassManager = academicClassManager;

        }
        // GET: StudentFeeAllocationsController
        public async Task<ActionResult> Index()
        {
            StudentFeeAllocationVM studentFeeAllocationVM = new StudentFeeAllocationVM();            
            studentFeeAllocationVM.StudentFeeAllocations = (System.Collections.Generic.List<Entities.StudentFeeAllocation>)await _studentFeeAllocationManager.GetAllAsync();
            studentFeeAllocationVM.FeeList = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
            studentFeeAllocationVM.AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            
            return View(studentFeeAllocationVM);
        }

        // GET: StudentFeeAllocationsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StudentFeeAllocationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentFeeAllocationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentFeeAllocationVM studentFeeAllocationVM)
        {
            StudentFeeAllocation studentFeeAllocation = new StudentFeeAllocation();
            studentFeeAllocation = studentFeeAllocationVM.SFAllocation;
            if (ModelState.IsValid)
            {
                studentFeeAllocation.CreatedAt = DateTime.Now;
                studentFeeAllocation.CreatedBy = HttpContext.Session.GetString("UserId");
                studentFeeAllocation.MACAddress = MACService.GetMAC();
                try
                {
                    bool isSaved = await _studentFeeAllocationManager.AddAsync(studentFeeAllocation);
                    if (isSaved)
                    {
                        TempData["success"] = "New Fee allocation added successfully";
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
         return RedirectToAction("Index");
           
        }

        // GET: StudentFeeAllocationsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StudentFeeAllocationsController/Edit/5
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

        // GET: StudentFeeAllocationsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StudentFeeAllocationsController/Delete/5
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
