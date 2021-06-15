using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DB;
using SMS.Entities;

namespace SchoolManagementSystem.Controllers
{
    public class BloodGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BloodGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.BloodGroup.ToListAsync());
        }

        // GET: BloodGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloodGroup = await _context.BloodGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodGroup == null)
            {
                return NotFound();
            }

            return View(bloodGroup);
        }

        // GET: BloodGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BloodGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] BloodGroup bloodGroup)
        {
            string msg = "";
            var existBG =await _context.BloodGroup.FirstOrDefaultAsync(s => s.Name.Trim() == bloodGroup.Name.Trim());
            if (existBG!=null)
            {
                msg = bloodGroup.Name + " is already exist.";
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    bloodGroup.CreatedAt = DateTime.Now;
                    bloodGroup.CreatedBy = HttpContext.Session.GetString("User");

                    _context.Add(bloodGroup);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(bloodGroup);
        }

        // GET: BloodGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloodGroup = await _context.BloodGroup.FindAsync(id);
            if (bloodGroup == null)
            {
                return NotFound();
            }
            return View(bloodGroup);
        }

        // POST: BloodGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] BloodGroup bloodGroup)
        {
            if (id != bloodGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    bloodGroup.EditedAt = DateTime.Now;
                    bloodGroup.EditedBy = HttpContext.Session.GetString("User");

                    _context.Update(bloodGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BloodGroupExists(bloodGroup.Id))
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
            return View(bloodGroup);
        }

        // GET: BloodGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloodGroup = await _context.BloodGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bloodGroup == null)
            {
                return NotFound();
            }

            return View(bloodGroup);
        }

        // POST: BloodGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bloodGroup = await _context.BloodGroup.FindAsync(id);
            _context.BloodGroup.Remove(bloodGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BloodGroupExists(int id)
        {
            return _context.BloodGroup.Any(e => e.Id == id);
        }
    }
}
