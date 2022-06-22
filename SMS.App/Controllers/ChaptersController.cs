using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.BLL.Contracts;
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
    }
}
