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

    [Authorize(Roles = "SuperAdmin, Admin")]
    public class DivisionsController : Controller
    {
        private readonly IDivisionManager _divisionManager;

        public DivisionsController(IDivisionManager divisionManager)
        {
            _divisionManager = divisionManager;
        }

        // GET: Divisions
        public async Task<IActionResult> Index()
        {
            return View(await _divisionManager.GetAllAsync());
        }

        // GET: Divisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _divisionManager.GetByIdAsync((int)id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // GET: Divisions/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Division division)
        {
            if (ModelState.IsValid)
            {
                await _divisionManager.AddAsync(division);
                return RedirectToAction(nameof(Index));
            }
            return View(division);
        }

        // GET: Divisions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _divisionManager.GetByIdAsync((int)id);
            if (division == null)
            {
                return NotFound();
            }
            return View(division);
        }

        // POST: Divisions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Division division)
        {
            if (id != division.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    await _divisionManager.UpdateAsync(division);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.Id))
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
            return View(division);
        }

        // GET: Divisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var division = await _divisionManager.GetByIdAsync((int)id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var division = await _divisionManager.GetByIdAsync(id);

            await _divisionManager.RemoveAsync(division);
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(int id)
        {
            var division = _divisionManager.GetByIdAsync(id);
            if (division!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("/api/divisions/allDivisions")]
        public async Task<IReadOnlyCollection<Division>> allDivisions()
        {
            return await _divisionManager.GetAllAsync();
        }
    }
}
