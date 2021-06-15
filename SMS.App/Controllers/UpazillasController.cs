using SMS.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class UpazilasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UpazilasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Upazilas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Upazila.Include(u => u.District);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Upazilas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _context.Upazila
                .Include(u => u.District)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Upazila == null)
            {
                return NotFound();
            }

            return View(Upazila);
        }

        // GET: Upazilas/Create
        public IActionResult Create()
        {
            ViewData["DistrictId"] = new SelectList(_context.District, "Id", "Id");
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
                _context.Add(Upazila);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(_context.District, "Id", "Id", Upazila.DistrictId);
            return View(Upazila);
        }

        // GET: Upazilas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _context.Upazila.FindAsync(id);
            if (Upazila == null)
            {
                return NotFound();
            }
            ViewData["DistrictId"] = new SelectList(_context.District, "Id", "Id", Upazila.DistrictId);
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
                    _context.Update(Upazila);
                    await _context.SaveChangesAsync();
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
            ViewData["DistrictId"] = new SelectList(_context.District, "Id", "Id", Upazila.DistrictId);
            return View(Upazila);
        }

        // GET: Upazilas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Upazila = await _context.Upazila
                .Include(u => u.District)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var Upazila = await _context.Upazila.FindAsync(id);
            _context.Upazila.Remove(Upazila);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpazilaExists(int id)
        {
            return _context.Upazila.Any(e => e.Id == id);
        }
    }
}
