using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    [Authorize]
    public class DistrictsController : Controller
    {
        private readonly IDistrictManager _districtManager;
        private readonly IDivisionManager _divisionManager;

        public DistrictsController(IDistrictManager districtManager, IDivisionManager divisionManager)
        {
            _districtManager = districtManager;
            _divisionManager = divisionManager;
        }

        // GET: Districts
        public async Task<IActionResult> Index()
        {
            var districts =await _districtManager.GetAllAsync() ;
            return View(districts);
        }

        // GET: Districts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = await _districtManager.GetByIdAsync((int)id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        // GET: Districts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DivisionId"] = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Id");
            return View();
        }

        // POST: Districts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DivisionId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] District district)
        {
            if (ModelState.IsValid)
            {
                await _districtManager.AddAsync(district);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Id", district.DivisionId);
            return View(district);
        }

        // GET: Districts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = await _districtManager.GetByIdAsync((int)id);
            if (district == null)
            {
                return NotFound();
            }
            ViewData["DivisionId"] = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Id", district.DivisionId);
            return View(district);
        }

        // POST: Districts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DivisionId,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] District district)
        {
            if (id != district.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _districtManager.UpdateAsync(district);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictExists(district.Id))
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
            ViewData["DivisionId"] = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Id", district.DivisionId);
            return View(district);
        }

        // GET: Districts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = await _districtManager.GetByIdAsync((int)id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var district = await _districtManager.GetByIdAsync((int)id);

            await _districtManager.RemoveAsync(district);
            return RedirectToAction(nameof(Index));
        }

        private bool DistrictExists(int id)
        {
            var district = _districtManager.GetByIdAsync(id);
            if (district != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("api/Districts/byDivision")]
        public async Task<IReadOnlyCollection<District>> GetDistrictByDivisionId(int divId)
        {
            return await _districtManager.GetAllByDivId(divId);
        }
    }
}
