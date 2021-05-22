﻿using System;
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
    public class AttachDocsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttachDocsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttachDocs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AttachDocs.Include(a => a.Employee).Include(a => a.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AttachDocs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachDoc = await _context.AttachDocs
                .Include(a => a.Employee)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attachDoc == null)
            {
                return NotFound();
            }

            return View(attachDoc);
        }

        // GET: AttachDocs/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "EmployeeName");
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name");
            return View();
        }

        // POST: AttachDocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DocumentsName,Image,StudentId,EmployeeId")] AttachDoc attachDoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attachDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "EmployeeName", attachDoc.EmployeeId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", attachDoc.StudentId);
            return View(attachDoc);
        }

        // GET: AttachDocs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachDoc = await _context.AttachDocs.FindAsync(id);
            if (attachDoc == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "EmployeeName", attachDoc.EmployeeId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", attachDoc.StudentId);
            return View(attachDoc);
        }

        // POST: AttachDocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentsName,Image,StudentId,EmployeeId")] AttachDoc attachDoc)
        {
            if (id != attachDoc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attachDoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttachDocExists(attachDoc.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "EmployeeName", attachDoc.EmployeeId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", attachDoc.StudentId);
            return View(attachDoc);
        }

        // GET: AttachDocs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachDoc = await _context.AttachDocs
                .Include(a => a.Employee)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attachDoc == null)
            {
                return NotFound();
            }

            return View(attachDoc);
        }

        // POST: AttachDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attachDoc = await _context.AttachDocs.FindAsync(id);
            _context.AttachDocs.Remove(attachDoc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttachDocExists(int id)
        {
            return _context.AttachDocs.Any(e => e.Id == id);
        }
    }
}
