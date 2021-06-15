using SMS.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class ReligionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReligionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Religions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Religion.ToListAsync());
        }

        // GET: Religions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var religion = await _context.Religion
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var religionExist = await _context.Religion.FirstOrDefaultAsync(r => r.Name.Trim() == religion.Name.Trim());
            string msg = "";
            if (religionExist!=null)
            {
                msg = religion.Name + " is already exist.";
                ViewBag.msg=msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    religion.CreatedAt = DateTime.Now;
                    religion.CreatedBy = HttpContext.Session.GetString("User");

                    _context.Add(religion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
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

            var religion = await _context.Religion.FindAsync(id);
            if (religion == null)
            {
                return NotFound();
            }
            return View(religion);
        }

        // POST: Religions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] Religion religion)
        {
            if (id != religion.Id)
            {
                return NotFound();
            }
            var religionExist = await _context.Religion.FirstOrDefaultAsync(r => r.Name.Trim() == religion.Name.Trim() && r.Id!=religion.Id);
            if (religionExist!=null)
            {
                ViewBag.msg = religion.Name + " is already exist.";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        religion.EditedAt = DateTime.Now;
                        religion.EditedBy = HttpContext.Session.GetString("User");

                        _context.Update(religion);
                        await _context.SaveChangesAsync();
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

            var religion = await _context.Religion
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var religion = await _context.Religion.FindAsync(id);
            _context.Religion.Remove(religion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReligionExists(int id)
        {
            return _context.Religion.Any(e => e.Id == id);
        }
    }
}
