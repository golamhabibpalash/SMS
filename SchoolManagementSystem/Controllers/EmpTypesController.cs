using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DatabaseContext;
using Models;

namespace SchoolManagementSystem.Controllers
{
    public class EmpTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmpTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmpType.ToListAsync());
        }

        // GET: EmpTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empType = await _context.EmpType
                .FirstOrDefaultAsync(m => m.Id == id);
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

        // POST: EmpTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedBy,CreatedAt,EditedBy,EditedAt")] EmpType empType)
        {
            string msg = "";
            var typeExist =await _context.EmpType.FirstOrDefaultAsync(e => e.Name.Trim() == empType.Name.Trim());
            if (typeExist!=null)
            {
                msg = empType.Name + " is already exist.";
                ViewBag.msg = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    empType.CreatedAt = DateTime.Now;
                    empType.CreatedBy = HttpContext.Session.GetString("User");

                    _context.Add(empType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
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

            var empType = await _context.EmpType.FindAsync(id);
            if (empType == null)
            {
                return NotFound();
            }
            return View(empType);
        }

        // POST: EmpTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedBy,CreatedAt,EditedBy,EditedAt")] EmpType empType)
        {
            if (id != empType.Id)
            {
                return NotFound();
            }
            string msg = "";
            var typeExist = await _context.EmpType.FirstOrDefaultAsync(e => e.Name.Trim() == empType.Name.Trim() && e.Id!=id);
            if (typeExist != null)
            {
                msg = empType.Name + " is already exist.";
                ViewBag.msg = msg;
            }
            if (ModelState.IsValid)
            {
                try
                {

                    empType.EditedAt = DateTime.Now;
                    empType.EditedBy = HttpContext.Session.GetString("User");
                    _context.Update(empType);
                    await _context.SaveChangesAsync();
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

            var empType = await _context.EmpType
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var empType = await _context.EmpType.FindAsync(id);
            _context.EmpType.Remove(empType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpTypeExists(int id)
        {
            return _context.EmpType.Any(e => e.Id == id);
        }
    }
}
