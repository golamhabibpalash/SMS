using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SMS.App.Controllers
{
    public class OffDaysControllercs : Controller
    {
        // GET: OffDaysControllercs
        public ActionResult Index()
        {
            return View();
        }

        // GET: OffDaysControllercs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OffDaysControllercs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OffDaysControllercs/Create
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

        // GET: OffDaysControllercs/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OffDaysControllercs/Edit/5
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

        // GET: OffDaysControllercs/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OffDaysControllercs/Delete/5
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
