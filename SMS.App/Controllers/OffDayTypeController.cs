using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class OffDayTypeController : Controller
    {
        // GET: OffDayTypeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: OffDayTypeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OffDayTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OffDayTypeController/Create
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

        // GET: OffDayTypeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OffDayTypeController/Edit/5
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

        // GET: OffDayTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OffDayTypeController/Delete/5
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
