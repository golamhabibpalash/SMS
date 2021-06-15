﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using SMS.BLL.Contracts;

namespace SMS.App.Controllers
{
    public class AcademicClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAcademicClassManager _academicClassManager;

        public AcademicClassesController(ApplicationDbContext context, IAcademicClassManager academicClassManager)
        {
            _context = context;
            _academicClassManager = academicClassManager;
        }

        // GET: AcademicClasses
        public async Task<IActionResult> Index()
        {
            var AcademicClassList =await _academicClassManager.GetAllAsync();

            return View(AcademicClassList);
        }

        // GET: AcademicClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicClass = await _academicClassManager.GetByIdAsync(Convert.ToInt32(id));
            if (academicClass == null)
            {
                return NotFound();
            }

            return View(academicClass);
        }

        // GET: AcademicClasses/Create
        public IActionResult Create()
        {
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            return View();
        }

        // POST: AcademicClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AcademicSessionId,ClassSerial,Description,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicClass academicClass)
        {
            string msg = "";
            if (academicClass.Name!=null)
            {
                if (ModelState.IsValid)
                {
                    var classExist = _context.AcademicClass.FirstOrDefault(c => c.Name.Trim() == academicClass.Name.Trim());
                    if (classExist != null)
                    {
                        msg = academicClass.Name + " name is already exist.";
                    }
                    else
                    {
                        academicClass.CreatedBy = HttpContext.Session.GetString("UserId");
                        academicClass.CreatedAt = DateTime.Today;

                        _context.Add(academicClass);
                        await _context.SaveChangesAsync();
                        TempData["create"] = "Created Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                msg = "Please insert a class name.";
            }
            ViewBag.msg = msg;
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            return View(academicClass);
        }

        // GET: AcademicClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicClass = await _context.AcademicClass.FindAsync(id);
            if (academicClass == null)
            {
                return NotFound();
            }
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            return View(academicClass);
        }

        // POST: AcademicClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AcademicSessionId,ClassSerial,Description,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicClass academicClass)
        {
            string msg = "";
            if (id != academicClass.Id)
            {
                return NotFound();
            }
            var existAcademicClass = _context.AcademicClass.Where(ac => ac.Name == academicClass.Name.Trim() && ac.Id != id).FirstOrDefault();
            if (existAcademicClass==null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        academicClass.EditedBy = HttpContext.Session.GetString("UserId");
                        academicClass.EditedAt = DateTime.Now;

                        _context.Update(academicClass);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AcademicClassExists(academicClass.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["edit"] = "Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                msg = "This class name is already exist in this session";
            }

            ViewBag.msg = msg;
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            return View(academicClass);
        }

        // GET: AcademicClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicClass = await _context.AcademicClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (academicClass == null)
            {
                return NotFound();
            }

            return View(academicClass);
        }

        // POST: AcademicClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicClass = await _context.AcademicClass.FindAsync(id);
            _context.AcademicClass.Remove(academicClass);
            await _context.SaveChangesAsync();
            TempData["delete"] = "Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicClassExists(int id)
        {
            return _context.AcademicClass.Any(e => e.Id == id);
        }
    }
}
