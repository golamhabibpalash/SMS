using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class AcademicExamController : Controller
    {
        // GET: AcademicExamController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AcademicExamController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AcademicExamController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademicExamController/Create
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

        // GET: AcademicExamController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AcademicExamController/Edit/5
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

        // GET: AcademicExamController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicExamController/Delete/5
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
