using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    public class EmpTypesController : Controller
    {
        private readonly IEmpTypeManager _empTypeManager;

        public EmpTypesController(IEmpTypeManager empTypeManager)
        {
            _empTypeManager = empTypeManager;
        }

        // GET: EmpTypes
        public async Task<IActionResult> Index()
        {
            return View(await _empTypeManager.GetAllAsync());
        }

        // GET: EmpTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empType = await _empTypeManager.GetByIdAsync((int)id);
            if (empType == null)
            {
                return NotFound();
            }

            return View(empType);
        }

        // GET: EmpTypes/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedBy,CreatedAt,EditedBy,EditedAt")] EmpType empType)
        {
            
            if (ModelState.IsValid)
            {
                empType.CreatedAt = DateTime.Now;
                empType.CreatedBy = HttpContext.Session.GetString("User");

                await _empTypeManager.AddAsync(empType);
                return RedirectToAction(nameof(Index));
            }
            return View(empType);
        }

        // GET: EmpTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empType = await _empTypeManager.GetByIdAsync((int) id);
            if (empType == null)
            {
                return NotFound();
            }
            return View(empType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedBy,CreatedAt,EditedBy,EditedAt")] EmpType empType)
        {
            if (id != empType.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {

                    empType.EditedAt = DateTime.Now;
                    empType.EditedBy = HttpContext.Session.GetString("User");
                    await _empTypeManager.UpdateAsync(empType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpTypeExists(empType.Id))
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
            return View(empType);
        }

        // GET: EmpTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empType = await _empTypeManager.GetByIdAsync((int)id);
            if (empType == null)
            {
                return NotFound();
            }

            return View(empType);
        }

        // POST: EmpTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empType = await _empTypeManager.GetByIdAsync((int)id);

            await _empTypeManager.RemoveAsync(empType);
            return RedirectToAction(nameof(Index));
        }

        private bool EmpTypeExists(int id)
        {
            var empType = _empTypeManager.GetByIdAsync((int)id);
            if (empType != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
