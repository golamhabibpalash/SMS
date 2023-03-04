using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AcademicExamTypeController : Controller
    {
        private readonly IAcademicExamTypeManager _academicExamTypeManager;
        public AcademicExamTypeController(IAcademicExamTypeManager academicExamTypeManager)
        {
            _academicExamTypeManager = academicExamTypeManager;
        }
        // GET: AcademicExamTypeController
        public async Task<ActionResult> Index()
        {
            var allExamTypes = await _academicExamTypeManager.GetAllAsync();
            return View(allExamTypes);
        }

        // GET: AcademicExamTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            AcademicExamType academicExamType = await _academicExamTypeManager.GetByIdAsync(id);
            return View(academicExamType);
        }

        // GET: AcademicExamTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademicExamTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AcademicExamType academicExamType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isSaved = await _academicExamTypeManager.AddAsync(academicExamType);
                    if (isSaved)
                    {
                        TempData["create"] = "Created Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["failed"] = "Failed! ";
                    return View(academicExamType);
                }

                return View(academicExamType);
                
            }
            catch
            {
                return View();
            }
        }

        // GET: AcademicExamTypeController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            AcademicExamType academicExamType = await _academicExamTypeManager.GetByIdAsync(id);
            return View(academicExamType);
        }

        // POST: AcademicExamTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AcademicExamType academicExamType)
        {
            try
            {
                if (id !=academicExamType.Id)
                {
                    return View(academicExamType);
                }
                if (ModelState.IsValid)
                {
                    var isSaved = await _academicExamTypeManager.UpdateAsync(academicExamType);
                    if (isSaved)
                    {
                        TempData["updated"] = "Updated Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(academicExamType);
            }
            catch
            {
                return View();
            }
        }

        // GET: AcademicExamTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicExamTypeController/Delete/5
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
