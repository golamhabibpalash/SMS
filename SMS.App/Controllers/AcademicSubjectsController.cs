using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.App.Utilities.MACIPServices;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AcademicSubjectsController : Controller
    {
        private readonly IAcademicSubjectManager _academicSubjectManager;
        private readonly IAcademicSubjectTypeManager _academicSubjectTypeManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IQuestionFormationManager _questionFormationManager;
        //private readonly ILogger<AcademicSubjectsController> logger;

        public AcademicSubjectsController(IAcademicSubjectManager academicSubjectManger, IAcademicSubjectTypeManager academicSubjectTypeManager, IAcademicClassManager academicClassManager, ILogger<AcademicSubjectsController> _Logger, IQuestionFormationManager questionFormationManager)
        {
            _academicSubjectManager = academicSubjectManger;
            _academicSubjectTypeManager = academicSubjectTypeManager;
            _academicClassManager = academicClassManager;
            _questionFormationManager = questionFormationManager;
            //logger = _Logger;
        }

        // GET: AcademicSubjects
        public async Task<IActionResult> Index()
        {
            ViewData["AcademicSubjectTypeId"] = new SelectList(await _academicSubjectTypeManager.GetAllAsync(), "Id", "SubjectTypeName");
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            ViewData["QuestionFormatId"] = new SelectList(await _questionFormationManager.GetAllAsync(), "Id", "Name");
            var academicSubject = await _academicSubjectManager.GetAllAsync();
            return View(academicSubject);
        }

        // GET: AcademicSubjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _academicSubjectManager.GetByIdAsync((int)id);
            if (academicSubject == null)
            {
                return NotFound();
            }

            return View(academicSubject);
        }

        // GET: AcademicSubjects/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AcademicSubjectTypeId"] = new SelectList(await _academicSubjectTypeManager.GetAllAsync(), "Id", "SubjectTypeName");
            ViewData["QuestionFormatId"] = new SelectList(await _questionFormationManager.GetAllAsync(), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectName,AcademicSubjectTypeId,AcademicClassId,SubjectCode,SubjectFor,TotalMarks,Status,CreatedBy,CreatedAt,EditedBy,EditedAt,QuestionFormatId")] AcademicSubject academicSubject)
        {
            academicSubject.Status = true;
            if (ModelState.IsValid)
            {
                academicSubject.MACAddress = MACService.GetMAC();
                academicSubject.CreatedAt = DateTime.Now;
                academicSubject.CreatedBy = HttpContext.Session.GetString("UserId");
                academicSubject.SubjectFor = 's';
                var isSaved = await _academicSubjectManager.AddAsync(academicSubject);
                if (isSaved)
                {
                    TempData["created"] = "Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Already Exist";
                }
            }

            ViewData["AcademicSubjectTypeId"] = new SelectList(await _academicSubjectTypeManager.GetAllAsync(), "Id", "SubjectTypeName", academicSubject.AcademicSubjectTypeId);
            ViewData["QuestionFormatId"] = new SelectList(await _questionFormationManager.GetAllAsync(), "Id", "Name", academicSubject.QuestionFormatId);
            return View(academicSubject);
        }

        // GET: AcademicSubjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _academicSubjectManager.GetByIdAsync((int)id);
            if (academicSubject == null)
            {
                return NotFound();
            }
            ViewData["AcademicSubjectTypeId"] = new SelectList(await _academicSubjectTypeManager.GetAllAsync(), "Id", "SubjectTypeName", academicSubject.AcademicSubjectTypeId);
            ViewData["QuestionFormatId"] = new SelectList(await _questionFormationManager.GetAllAsync(), "Id", "Name", academicSubject.QuestionFormatId);
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicSubject.AcademicClassId);
            return View(academicSubject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectName,AcademicSubjectTypeId,SubjectCode,SubjectFor,TotalMarks,Status,CreatedBy,CreatedAt,EditedBy,EditedAt,QuestionFormatId,AcademicClassId")] AcademicSubject academicSubject)
        {
            if (id != academicSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    academicSubject.EditedAt = DateTime.Now;
                    academicSubject.EditedBy = HttpContext.Session.GetString("UserId");
                    academicSubject.MACAddress = MACService.GetMAC();

                    TempData["updated"] = "Updated Successfully";
                    await _academicSubjectManager.UpdateAsync(academicSubject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicSubjectExists(academicSubject.Id))
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
            ViewData["AcademicSubjectTypeId"] = new SelectList(await _academicSubjectTypeManager.GetAllAsync(), "Id", "SubjectTypeId", academicSubject.AcademicSubjectTypeId);
            ViewData["QuestionFormatId"] = new SelectList(await _questionFormationManager.GetAllAsync(), "Id", "Name", academicSubject.QuestionFormatId);
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", academicSubject.AcademicClassId);
            return View(academicSubject);
        }

        // GET: AcademicSubjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubject = await _academicSubjectManager.GetByIdAsync((int)id);
            if (academicSubject == null)
            {
                return NotFound();
            }

            return View(academicSubject);
        }

        // POST: AcademicSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicSubject = await _academicSubjectManager.GetByIdAsync(id);

            await _academicSubjectManager.RemoveAsync(academicSubject);
            TempData["deleted"] = "Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSubjectExists(int id)
        {
            var academicSubject = _academicSubjectManager.GetByIdAsync(id);
            if (academicSubject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<JsonResult> GetSubjectsByClassId(int classId)
        {
            var subjects = await _academicSubjectManager.GetSubjectsByClassIdAsync(classId);
            return Json(subjects);
        }
        
        public async Task<JsonResult> GetSubjectDetailsBySubjectId(int SubjectId)
        {
            var existingSubject = await _academicSubjectManager.GetByIdAsync(SubjectId);
            return Json(existingSubject);
        }
    }
}
