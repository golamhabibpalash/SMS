﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AcademicSubjectTypesController : Controller
    {
        private readonly IAcademicSubjectTypeManager _academicSubjectTypeManager;
        private readonly ILogger<AcademicSubjectTypesController> logger;

        public AcademicSubjectTypesController(IAcademicSubjectTypeManager academicSubjectTypeManager, ILogger<AcademicSubjectTypesController> _logger)
        {
            _academicSubjectTypeManager = academicSubjectTypeManager;
            logger = _logger;
        }

        // GET: AcademicSubjectTypes
        public async Task<IActionResult> Index()
        {
            
            return View(await _academicSubjectTypeManager.GetAllAsync());
        }

        // GET: AcademicSubjectTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubjectType = await _academicSubjectTypeManager.GetByIdAsync((int)id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }

            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SubjectTypeName,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubjectType academicSubjectType)
        {
            if (ModelState.IsValid)
            {
                academicSubjectType.CreatedAt = DateTime.Now;
                academicSubjectType.CreatedBy = HttpContext.Session.GetString("User");

                await _academicSubjectTypeManager.AddAsync(academicSubjectType);
                return RedirectToAction(nameof(Index));
            }
            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            

            var academicSubjectType = await _academicSubjectTypeManager.GetByIdAsync((int)id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }
            return View(academicSubjectType);
        }

        // POST: AcademicSubjectTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectTypeName,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSubjectType academicSubjectType)
        {
            if (id != academicSubjectType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    academicSubjectType.EditedAt = DateTime.Now;
                    academicSubjectType.EditedBy = HttpContext.Session.GetString("User");

                    await _academicSubjectTypeManager.UpdateAsync(academicSubjectType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicSubjectTypeExists(academicSubjectType.Id))
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
            return View(academicSubjectType);
        }

        // GET: AcademicSubjectTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicSubjectType = await _academicSubjectTypeManager.GetByIdAsync((int)id);
            if (academicSubjectType == null)
            {
                return NotFound();
            }

            return View(academicSubjectType);
        }

        // POST: AcademicSubjectTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var academicSubjectType = await _academicSubjectTypeManager.GetByIdAsync(id);
            
            await _academicSubjectTypeManager.RemoveAsync(academicSubjectType);
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSubjectTypeExists(int id)
        {
            var result = _academicSubjectTypeManager.GetByIdAsync(id);
            if (result != null)
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
