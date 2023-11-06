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
    public class StudentFeeHeadsController : Controller
    {
        private readonly IStudentFeeHeadManager _studentFeeHeadManager;
        private readonly IClassFeeListManager _classFeeListManager;
        private readonly IAcademicSessionManager _academicSessionManager;

        public StudentFeeHeadsController(IStudentFeeHeadManager studentFeeHeadManager, IClassFeeListManager classFeeListManager, IAcademicSessionManager academicSessionManager)
        {
            _studentFeeHeadManager = studentFeeHeadManager;
            _classFeeListManager = classFeeListManager;
            _academicSessionManager = academicSessionManager;
        }

        // GET: StudentFeeHeads
        public async Task<IActionResult> Index()
        {
            var feeHead = await _studentFeeHeadManager.GetAllAsync();
            return View(feeHead);
        }

        // GET: StudentFeeHeads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Repeatedly,YearlyFrequency,CreatedBy,CreatedAt,EditedBy,EditedAt,ContraFeeheadId,IsResidential")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            var sfhExist = await _studentFeeHeadManager.GetByNameAsync(studentFeeHead.Name);
            if (sfhExist!=null)
            {
                msg = studentFeeHead.Name+" Fee Head is already exist.";
                ViewBag.msg = msg;
                TempData["crateFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    studentFeeHead.CreatedBy = HttpContext.Session.GetString("UserId");
                    studentFeeHead.CreatedAt = DateTime.Now;
                    studentFeeHead.MACAddress = MACService.GetMAC();
                    await _studentFeeHeadManager.AddAsync(studentFeeHead);

                    TempData["Create"] = "Successfully Created";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["StudentFeeHeadList"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }
            return View(studentFeeHead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Repeatedly,YearlyFrequency,CreatedBy,CreatedAt,EditedBy,EditedAt,ContraFeeheadId,IsResidential")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            int isChanged = 0;
            
            if (id != studentFeeHead.Id)
            {
                return NotFound();
            }
            if (studentFeeHead.Id == studentFeeHead.ContraFeeheadId)
            {
                ViewData["StudentFeeHeadList"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name", studentFeeHead.ContraFeeheadId);
                msg = "Same Type and Contra Type will not be same.";
                ViewBag.msg = msg;
                TempData["editFail"] = msg;
                return View(studentFeeHead);
            }
            var sfhExist =await _studentFeeHeadManager.GetByIdAsync(id);

            if (sfhExist!=null)
            {
                if (studentFeeHead.Name != sfhExist.Name || studentFeeHead.Repeatedly != sfhExist.Repeatedly || studentFeeHead.YearlyFrequency != sfhExist.YearlyFrequency || studentFeeHead.ContraFeeheadId != sfhExist.ContraFeeheadId || studentFeeHead.IsResidential !=sfhExist.IsResidential)
                {
                    isChanged = 1;
                }
            }


            if (isChanged == 0)
            {
                msg = studentFeeHead.Name + " is already exists";
                ViewBag.msg = msg;
                TempData["editFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        studentFeeHead.EditedAt = DateTime.Now;
                        studentFeeHead.EditedBy = HttpContext.Session.GetString("UserId");
                        studentFeeHead.MACAddress = MACService.GetMAC();

                        bool isSaved = await _studentFeeHeadManager.UpdateAsync(studentFeeHead);
                        if (isSaved)
                        {


                            if (studentFeeHead.ContraFeeheadId > 0)
                            {
                                StudentFeeHead existingFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)studentFeeHead.ContraFeeheadId);
                                existingFeeHead.ContraFeeheadId = studentFeeHead.Id;
                                await _studentFeeHeadManager.UpdateAsync(existingFeeHead);
                            }
                            else
                            {
                                var feeHeads = await _studentFeeHeadManager.GetAllAsync();
                                StudentFeeHead existingFeeHead = feeHeads.FirstOrDefault(h => h.ContraFeeheadId == (int)studentFeeHead.Id);
                                if (existingFeeHead!=null)
                                {
                                    existingFeeHead.ContraFeeheadId = null;
                                    await _studentFeeHeadManager.UpdateAsync(existingFeeHead);
                                }
                            }
                            TempData["edit"] = "Edited Successfully.";
                        }
                    }
                    catch (Exception )
                    {
                        var StudentFeeHeadExists = await _studentFeeHeadManager.GetByIdAsync(id);
                        if (StudentFeeHeadExists==null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["StudentFeeHeadList"] = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name",studentFeeHead.ContraFeeheadId);
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // POST: StudentFeeHeads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync(id);
            await _studentFeeHeadManager.RemoveAsync(studentFeeHead);
            return RedirectToAction(nameof(Index));
        }
        public async Task<JsonResult> GetById(int id, int classId, int?sessionId)
        {
            if (sessionId==null)
            {
                AcademicSession academicSession = await _academicSessionManager.GetCurrentAcademicSession();
                sessionId = academicSession.Id;
            }
            var feeHead = await _studentFeeHeadManager.GetByIdAsync(id);
            var classFeeList = await _classFeeListManager.GetByClassIdAndFeeHeadIdAsync(classId, id,(int)sessionId);
            return Json(classFeeList);
        }
    }
}
