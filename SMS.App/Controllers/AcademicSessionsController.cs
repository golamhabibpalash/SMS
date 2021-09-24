using System;
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
    
    public class AcademicSessionsController : Controller
    {
        private readonly IAcademicSessionManager _sessionManager;
        private readonly ILogger<AcademicSessionsController> logger;

        public AcademicSessionsController(IAcademicSessionManager sessionManager, ILogger<AcademicSessionsController> _Logger)
        {
            _sessionManager = sessionManager;
            logger = _Logger;
        }

        // GET: AcademicSessions
        public async Task<IActionResult> Index()
        {
            //var aSession = await _context.AcademicSession.OrderBy(a => a.Name.Trim().Substring(0,4)).ToListAsync();
            var aSession = await _sessionManager.GetAllAsync();
            return View(aSession);
        }

        // GET: AcademicSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = Convert.ToInt32(id);

            //var academicSession = await _context.AcademicSession
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var academicSession = await _sessionManager.GetByIdAsync(myId);
            if (academicSession == null)
            {
                return NotFound();
            }

            return View(academicSession);
        }

        // GET: AcademicSessions/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSession academicSession)
        {
            string msg = "";
            if (academicSession.Name!=null)
            {
                if (ModelState.IsValid)
                {
                    //var existingSession = _context.AcademicSession.Where(s => s.Name.Trim() == academicSession.Name.Trim()).FirstOrDefault();

                    //if (existingSession != null)
                    //{
                    //    msg = "This name is already exists.";
                    //}
                    //else
                    //{
                        academicSession.CreatedAt = DateTime.Now;
                        academicSession.CreatedBy = HttpContext.Session.GetString("UserId");

                        //_context.Add(academicSession);
                    await _sessionManager.AddAsync(academicSession);
                        TempData["create"] = "Created Successfully";
                        return RedirectToAction(nameof(Index));
                    //}

                }
            }
            else
            {
                msg = "Please Input a name first.";
            }
            ViewBag.msg = msg;
            return View(academicSession);
        }

        // GET: AcademicSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var academicSession = await _context.AcademicSession.FindAsync(id);
            var academicSession = await _sessionManager.GetByIdAsync((int)id);
            if (academicSession == null)
            {
                return NotFound();
            }
            return View(academicSession);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status,CreatedBy,CreatedAt,EditedBy,EditedAt")] AcademicSession academicSession)
        {
            string msg = "";
            if (id != academicSession.Id)
            {
                return NotFound();
            }
            //var existStudent = _context.AcademicSession.FirstOrDefault(a => a.Name.Trim() == academicSession.Name.Trim() && a.Id != id);

                if (ModelState.IsValid)
                    {
                        try
                        {
                            academicSession.EditedBy = HttpContext.Session.GetString("UserId");
                            academicSession.EditedAt = DateTime.Now;

                            //_context.Update(academicSession);

                        await _sessionManager.UpdateAsync(academicSession);
                            
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!AcademicSessionExists(academicSession.Id))
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

            ViewBag.msg = msg;
            return View(academicSession);
        }

        // GET: AcademicSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var academicSession = await _context.AcademicSession
            //    .FirstOrDefaultAsync(m => m.Id == id);
            var academicSession = await _sessionManager.GetByIdAsync((int)id);
            if (academicSession == null)
            {
                return NotFound();
            }

            return View(academicSession);
        }

        // POST: AcademicSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var academicSession = await _context.AcademicSession.FindAsync(id);
            //_context.AcademicSession.Remove(academicSession);
            var academicSession = await _sessionManager.GetByIdAsync(id);
            await _sessionManager.RemoveAsync(academicSession);
            TempData["delete"] = "Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicSessionExists(int id)
        {
            var isExist =  _sessionManager.GetByIdAsync(id);
            if (isExist!=null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
