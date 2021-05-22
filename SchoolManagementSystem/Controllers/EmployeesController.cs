using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employee
                .Include(e => e.Designation)
                .Include(e => e.EmpType)
                .Include(e => e.Gender)
                .Include(e => e.Nationality)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PresentUpazila)
                .Include(e => e.Religion);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Designation)
                .Include(e => e.EmpType)
                .Include(e => e.Gender)
                .Include(e => e.Nationality)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PresentUpazila)
                .Include(e => e.Religion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "Name");
            ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Name");
            ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Name");
            ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "Name");
            ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Name");
            ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Name");
            ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Name");
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "Id", employee.DesignationId);
            ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name", employee.EmpTypeId);
            ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Id", employee.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Id", employee.NationalityId);
            ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PermanentDistrictId);
            ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PermanentDivisionId);
            ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PermanentUpazilaId);
            ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PresentDistrictId);
            ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PresentDivisionId);
            ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PresentUpazilaId);
            ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Id", employee.ReligionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "Id", employee.DesignationId);
            ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name", employee.EmpTypeId);
            ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Id", employee.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Id", employee.NationalityId);
            ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PermanentDistrictId);
            ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PermanentDivisionId);
            ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PermanentUpazilaId);
            ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PresentDistrictId);
            ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PresentDivisionId);
            ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PresentUpazilaId);
            ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Id", employee.ReligionId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Designation)
                .Include(e => e.EmpType)
                .Include(e => e.Gender)
                .Include(e => e.Nationality)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PresentUpazila)
                .Include(e => e.Religion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
