using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.App.Utilities.MACIPServices;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ClassFeeListsController : Controller
    {
        private readonly IClassFeeListManager _classFeeListManager;
        private readonly IStudentFeeHeadManager _studentFeeHeadManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAcademicSessionManager academicSessionManager;

        public ClassFeeListsController(IClassFeeListManager classFeeListManager, IStudentFeeHeadManager studentFeeHeadManager, IAcademicClassManager academicClassManager, IAcademicSessionManager academicSessionManager)
        {
            _classFeeListManager = classFeeListManager;
            _studentFeeHeadManager = studentFeeHeadManager;
            _academicClassManager = academicClassManager;
            this.academicSessionManager = academicSessionManager;
        }

        // GET: StudentFeeLists
        public async Task<IActionResult> Index()
        {
            string msg = "";
            if (TempData["success"]!=null)
            {
                msg = TempData["success"].ToString();
                ViewBag.msg = msg;
            }
            AcademicSession currentSession = await academicSessionManager.GetCurrentAcademicSession();
            ViewBag.currentSessionId = currentSession.Id;
            var result = await _classFeeListManager.GetAllAsync();
            
            return View(result.OrderByDescending(s => s.AcademicSessionId).ThenBy(m => m.AcademicClassId).ThenBy( s=> s.StudentFeeHeadId));
        }

        // GET: StudentFeeLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeList = await _classFeeListManager.GetByIdAsync((int)id);
            if (studentFeeList == null)
            {
                return NotFound();
            }

            return View(studentFeeList);
        }

        // GET: StudentFeeLists/Create
        public async Task<IActionResult> Create()
        {
            
            ViewData["StudentFeeHeadId"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync() , "Id", "Name");
            ViewData["AcademicSessionId"] = new SelectList(await academicSessionManager.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            string msg = "";

            var feeListExist = await _classFeeListManager.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(classFeeList.AcademicClassId, classFeeList.StudentFeeHeadId,classFeeList.AcademicSessionId);

            if (feeListExist!=null && feeListExist.Count>0)
            {
                msg = "Fee list for this class is already exists.";
                TempData["crateFail"] = msg;
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    classFeeList.CreatedAt = DateTime.Now;
                    classFeeList.CreatedBy = HttpContext.Session.GetString("UserId");
                    classFeeList.MACAddress = MACService.GetMAC();
                    bool isSaved = await _classFeeListManager.AddAsync(classFeeList);
                    if (isSaved)
                    {
                        msg = "Saved Successfully.";
                        TempData["success"] = msg;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }


            ViewData["AcademicSessionId"] = new SelectList(await academicSessionManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicSessionId);
            ViewData["StudentFeeHeadId"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name",classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicClassId);

            return View(classFeeList);
        }

        // GET: StudentFeeLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classFeeList = await _classFeeListManager.GetByIdAsync((int)id);
            if (classFeeList == null)
            {
                return NotFound();
            }

            ViewData["AcademicSessionId"] = new SelectList(await academicSessionManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicSessionId);
            ViewData["StudentFeeHeadId"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name", classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicClassId);
            return View(classFeeList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            if (id != classFeeList.Id)
            {
                return NotFound();
            }
            

            if (ModelState.IsValid)
            {
                try
                {
                    classFeeList.EditedAt = DateTime.Now;
                    classFeeList.EditedBy = HttpContext.Session.GetString("UserId");
                    classFeeList.MACAddress = MACService.GetMAC();
                    bool isUpdated = await _classFeeListManager.UpdateAsync(classFeeList);
                    if (isUpdated==true)
                    {
                        TempData["edit"] = "Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentFeeListExists(classFeeList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["fail"] = "Fail to Update";
                return View();
            }
           
            return View(classFeeList);
        }

        // GET: StudentFeeLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeList = await _classFeeListManager.GetByIdAsync((int)id);
            if (studentFeeList == null)
            {
                return NotFound();
            }

            return View(studentFeeList);
        }

        // POST: StudentFeeLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classFeeList = await _classFeeListManager.GetByIdAsync(id);
            bool isDeleted = await _classFeeListManager.RemoveAsync(classFeeList);
            
            if (isDeleted)
            {
                TempData["delete"] = "Successfully Deleted";
                return RedirectToAction(nameof(Index));
            }
            return View();
            
        }

        private bool StudentFeeListExists(int id)
        {
            var classFeeList =  _classFeeListManager.GetByIdAsync(id);
            if (classFeeList!=null)
            {
                return true;
            }
            return false;
        }
    }
}
