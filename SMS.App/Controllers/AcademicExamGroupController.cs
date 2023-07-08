using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AcademicExamGroupController : Controller
    {
        private readonly IAcademicExamGroupManager _examGroupManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly IAcademicExamTypeManager _academicExamTypeManager;
        public AcademicExamGroupController(IAcademicExamGroupManager examGroupManager, IAcademicSessionManager academicSessionManager, IAcademicExamTypeManager academicExamTypeManager)
        {
            _examGroupManager = examGroupManager;
            _academicSessionManager = academicSessionManager;
            _academicExamTypeManager = academicExamTypeManager;

        }
        // GET: AcademicExamGroupController
        public async Task<ActionResult> Index()
        {
            List<AcademicExamGroup> result = new List<AcademicExamGroup>();
            try
            {
                result = (List<AcademicExamGroup>)await _examGroupManager.GetAllAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
            return View(result);
        }

        // GET: AcademicExamGroupController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AcademicExamGroupController/Create
        public async Task<ActionResult> Create()
        {
            ViewData["AcademicSessionList"] = new SelectList(await _academicSessionManager.GetAllAsync(),"Id","Name");
            ViewData["AcademicExamTypeList"] = new SelectList(await _academicExamTypeManager.GetAllAsync(),"Id", "ExamTypeName");
            return View();
        }

        // POST: AcademicExamGroupController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AcademicExamGroup academicExamGroup)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Something wrong";
                return View(academicExamGroup);
            }
            try
            {
                academicExamGroup.CreatedAt = DateTime.Now;
                academicExamGroup.CreatedBy = HttpContext.Session.GetString("UserId");
                academicExamGroup.MACAddress = MACService.GetMAC();
                academicExamGroup.Status = true;
                bool isSaved = await _examGroupManager.AddAsync(academicExamGroup);
                if (!isSaved)
                {
                    TempData["error"] = "Something during save time";
                }
                else
                {
                    TempData["created"] = "Alhamdulillah! Exam Group Successfully Created";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                throw;
            }

            ViewData["AcademicSessionList"] = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name",academicExamGroup.AcademicSessionId);
            ViewData["AcademicExamTypeList"] = new SelectList(await _academicExamTypeManager.GetAllAsync(), "Id", "ExamTypeName",academicExamGroup.academicExamTypeId);
            return View(academicExamGroup);
        }

        // GET: AcademicExamGroupController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AcademicExamGroupController/Edit/5
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

        // GET: AcademicExamGroupController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicExamGroupController/Delete/5
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
