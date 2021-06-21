using SMS.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using SMS.BLL.Contracts;

namespace SchoolManagementSystem.Controllers
{

    public class ReligionsController : Controller
    {
        private readonly IReligionManager _religionManager;

        public ReligionsController(ApplicationDbContext context, IReligionManager religionManager)
        {
            _religionManager = religionManager;
        }

        // GET: Religions
        public async Task<IActionResult> Index()
        {
            return View(await _religionManager.GetAllAsync());
        }

        // GET: Religions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var religion = await _religionManager.GetByIdAsync((int)id);
            if (religion == null)
            {
                return NotFound();
            }

            return View(religion);
        }

        // GET: Religions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Religions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Religion religion)
        {
            if (ModelState.IsValid)
            {
                religion.CreatedAt = DateTime.Now;
                religion.CreatedBy = HttpContext.Session.GetString("User");

                await _religionManager.AddAsync(religion);
                return RedirectToAction(nameof(Index));
            }
          
            return View(religion);
        }

        // GET: Religions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var religion = await _religionManager.GetByIdAsync((int)id);
            if (religion == null)
            {
                return NotFound();
            }
            return View(religion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Religion religion)
        {
            if (id != religion.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    religion.EditedAt = DateTime.Now;
                    religion.EditedBy = HttpContext.Session.GetString("User");

                    await _religionManager.UpdateAsync(religion);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReligionExists(religion.Id))
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
            
            return View(religion);
        }

        // GET: Religions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var religion = await _religionManager.GetByIdAsync((int)id);
            if (religion == null)
            {
                return NotFound();
            }

            return View(religion);
        }

        // POST: Religions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var religion = await _religionManager.GetByIdAsync((int)id);

            await _religionManager.RemoveAsync(religion);
            return RedirectToAction(nameof(Index));
        }

        private bool ReligionExists(int id)
        {
            var religion = _religionManager.GetByIdAsync(id);
            if (religion!=null)
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
