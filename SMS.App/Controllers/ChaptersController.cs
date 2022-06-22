using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class ChaptersController : Controller
    {
        private readonly IChapterManager _chapterManager;
        private readonly IAcademicClassManager _academicClassManager;
        public ChaptersController(IChapterManager _chapterManager, IAcademicClassManager academicClassManager)
        {
            this._chapterManager = _chapterManager;
            _academicClassManager = academicClassManager;
        }
        public async Task<IActionResult> Index()
        {
            var chapters = await _chapterManager.GetAllAsync();

            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            return View(chapters);
        }

        [HttpPost]
        public async Task<JsonResult> Create(Chapter chapter)
        {
            try
            {
                chapter.CreatedAt = DateTime.Now;
                chapter.CreatedBy = HttpContext.Session.GetString("UserId");
                var isSaved =await _chapterManager.AddAsync(chapter);
                if (!isSaved)
                {
                    return Json(new {errorMsg ="Not Saved"});
                }
                Chapter newChapter = await _chapterManager.GetByIdAsync(chapter.Id);
                return Json(newChapter);
            }
            catch (System.Exception)
            {
                throw;
            }
            
        }
    }
}
