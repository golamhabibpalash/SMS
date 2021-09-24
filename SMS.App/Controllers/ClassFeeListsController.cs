using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
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
            }
            var result = await _classFeeListManager.GetAllAsync();
            ViewBag.msg = msg;
            return View(result);
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

        // POST: StudentFeeLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            string msg = "";


            var feeListExist = await _classFeeListManager.GetByClassIdAndFeeHeadIdAsync(classFeeList.AcademicClassId, classFeeList.StudentFeeHeadId);

            if (feeListExist!=null)
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
                    
                    bool isSaved = await _classFeeListManager.AddAsync(classFeeList);
                    if (isSaved)
                    {
                        msg = "Saved Successfully.";
                    }
                    ViewBag.msg = msg;
                    TempData["create"] = msg;
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

        // POST: StudentFeeLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentFeeHeadId,AcademicSessionId,Amount,AcademicClassId,CreatedBy,CreatedAt,EditedBy,EditedAt")] ClassFeeList classFeeList)
        {
            if (id != classFeeList.Id)
            {
                return NotFound();
            }
            var feeList = await _classFeeListManager.GetAllAsync();
            var feeListExist =feeList.Where(s => s.AcademicClassId == classFeeList.AcademicClassId && s.Id !=id).FirstOrDefault();

            if (feeListExist!=null)
            {
                TempData["editFail"] = "Failed";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        classFeeList.EditedAt = DateTime.Now;
                        classFeeList.EditedBy = HttpContext.Session.GetString("UserId");

                        bool isUpdated = await _classFeeListManager.UpdateAsync(classFeeList);
                        if (isUpdated)
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
            }


            ViewData["AcademicSessionId"] = new SelectList(await academicSessionManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicSessionId);
            ViewData["StudentFeeHeadId"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name", classFeeList.StudentFeeHeadId);
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", classFeeList.AcademicClassId);
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
