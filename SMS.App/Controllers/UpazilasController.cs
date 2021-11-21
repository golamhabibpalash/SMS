using SMS.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System.Linq;
using System.Threading.Tasks;
using SMS.BLL.Contracts;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace SMS.App.Controllers
{
    [Authorize]
    public class UpazilasController : Controller
    {
        private readonly IUpazilaManager _upazilaManager;
        private readonly IDistrictManager _districtManager;

        public UpazilasController(IUpazilaManager upazilaManager, IDistrictManager districtManager)
        {
            _upazilaManager = upazilaManager;
            _districtManager = districtManager;
        }

        // GET: Upazilas
        public async Task<IActionResult> Index()
        {

            return View(await _upazilaManager.GetAllAsync());
        }

        // GET: Upazilas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _upazilaManager.GetByIdAsync((int)id);
            if (Upazila == null)
            {
                return NotFound();
            }

            return View(Upazila);
        }

        // GET: Upazilas/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: Upazilas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DistrictId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Upazila Upazila)
        {
            if (ModelState.IsValid)
            {
                await _upazilaManager.AddAsync(Upazila);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", Upazila.DistrictId);
            return View(Upazila);
        }

        // GET: Upazilas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _upazilaManager.GetByIdAsync((int)id);
            if (Upazila == null)
            {
                return NotFound();
            }
            ViewData["DistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", Upazila.DistrictId);
            return View(Upazila);
        }

        // POST: Upazilas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DistrictId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Upazila Upazila)
        {
            if (id != Upazila.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _upazilaManager.UpdateAsync(Upazila);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpazilaExists(Upazila.Id))
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
            ViewData["DistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", Upazila.DistrictId);
            return View(Upazila);
        }

        // GET: Upazilas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _upazilaManager.GetByIdAsync((int)id);
            if (Upazila == null)
            {
                return NotFound();
            }

            return View(Upazila);
        }

        // POST: Upazilas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Upazila = await _upazilaManager.GetByIdAsync(id);

            await _upazilaManager.RemoveAsync(Upazila);
            return RedirectToAction(nameof(Index));
        }

        private bool UpazilaExists(int id)
        {
            var Upazila = _upazilaManager.GetByIdAsync((int)id);
            if (Upazila != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("api/upazilas/byDistrict")]
        public async Task<IReadOnlyCollection<Upazila>> GetByDistrictId(int id)
        {
            return await _upazilaManager.GetByDistrictId(id);
        }
    }
}
