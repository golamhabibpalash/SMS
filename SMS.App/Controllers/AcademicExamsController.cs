using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AcademicExamsController : Controller
    {
        private readonly IAcademicExamManager _examManager;
        private readonly IAcademicSessionManager _sessionManager;
        private readonly IAcademicClassManager _classManager;
        public AcademicExamsController(IAcademicExamManager examManager, IAcademicSessionManager sessionManager, IAcademicClassManager classManager)
        {
            _examManager = examManager;
            _sessionManager = sessionManager;
            _classManager = classManager;

        }

        // GET: AcademicExamsController
        public async Task<ActionResult> Index()
        {
            var exams = await _examManager.GetAllAsync();

            return View(exams);
        }

        // GET: AcademicExamsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var exam = await _examManager.GetByIdAsync(id);
            return View(exam);
        }

        // GET: AcademicExamsController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(),"Id","Name");
            ViewBag.AcademicClassList = new SelectList(await _classManager.GetAllAsync(),"Id","Name");
            return View();
        }

        // POST: AcademicExamsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcademicExam academicExam)
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

        // GET: AcademicExamsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AcademicExamsController/Edit/5
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

        // GET: AcademicExamsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicExamsController/Delete/5
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
