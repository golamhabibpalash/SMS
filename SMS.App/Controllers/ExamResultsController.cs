using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.BLL.Contracts;

namespace SMS.App.Controllers
{
    public class ExamResultsController : Controller
    {
        private readonly IExamResultManager _examResultManager;
        public ExamResultsController(IExamResultManager examResultManager)
        {
            _examResultManager = examResultManager;
        }
        // GET: ExamResultsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ExamResultsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExamResultsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExamResultsController/Create
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

        // GET: ExamResultsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExamResultsController/Edit/5
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

        // GET: ExamResultsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExamResultsController/Delete/5
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
