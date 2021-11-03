using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;

namespace SMS.App.Controllers
{
    public class StudentFeeHeadsController : Controller
    {
        private readonly IStudentFeeHeadManager _studentFeeHeadManager;

        public StudentFeeHeadsController(IStudentFeeHeadManager studentFeeHeadManager)
        {
            _studentFeeHeadManager = studentFeeHeadManager;
        }

        // GET: StudentFeeHeads
        public async Task<IActionResult> Index()
        {
            return View(await _studentFeeHeadManager.GetAllAsync());
        }

        // GET: StudentFeeHeads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Repeatedly,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            var sfhExist = await _studentFeeHeadManager.GetByNameAsync(studentFeeHead.Name);
            if (sfhExist!=null)
            {
                msg = studentFeeHead.Name+" Fee Head is already exist.";
                ViewBag.msg = msg;
                TempData["crateFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    studentFeeHead.CreatedBy = HttpContext.Session.GetString("User");
                    studentFeeHead.CreatedAt = DateTime.Now;

                    await _studentFeeHeadManager.AddAsync(studentFeeHead);

                    TempData["Create"] = "Successfully Created";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }
            return View(studentFeeHead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Repeatedly,CreatedBy,CreatedAt,EditedBy,EditedAt")] StudentFeeHead studentFeeHead)
        {
            string msg = "";
            if (id != studentFeeHead.Id)
            {
                return NotFound();
            }
            var sfhExist =await _studentFeeHeadManager.GetByIdAsync(id);

            if (sfhExist!=null)
            {
                msg = studentFeeHead.Name + " is already exists";
                ViewBag.msg = msg;
                TempData["editFail"] = msg;
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        studentFeeHead.EditedAt = DateTime.Now;
                        studentFeeHead.EditedBy = HttpContext.Session.GetString("User");


                        await _studentFeeHeadManager.UpdateAsync(studentFeeHead);
                        TempData["edit"] = "Edited Successfully.";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        var StudentFeeHeadExists = await _studentFeeHeadManager.GetByIdAsync(id);
                        if (StudentFeeHeadExists==null)
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
            return View(studentFeeHead);
        }

        // GET: StudentFeeHeads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync((int)id);
            if (studentFeeHead == null)
            {
                return NotFound();
            }

            return View(studentFeeHead);
        }

        // POST: StudentFeeHeads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentFeeHead = await _studentFeeHeadManager.GetByIdAsync(id);
            await _studentFeeHeadManager.RemoveAsync(studentFeeHead);
            return RedirectToAction(nameof(Index));
        }

    }
}
