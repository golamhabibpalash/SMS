using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.GradingTable;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class GradingTableController : Controller
    {
        private readonly IGradingTableManager _gradingTableManager;
        public GradingTableController(IGradingTableManager gradingTableManager)
        {
            _gradingTableManager = gradingTableManager;
        }


        public async Task<IActionResult> Index()
        {
            List<GradingIndexVM> gradingIndexVMs = new List<GradingIndexVM>(); 
            List<GradingTable> gradingTables = (List<GradingTable>)await _gradingTableManager.GetAllAsync();
            if (gradingTables!=null)
            {
                foreach (var item in gradingTables)
                {
                    GradingIndexVM gradingIndexVM = new GradingIndexVM();
                    gradingIndexVM.NumberRange = item.NumberRangeMin + " - " + item.NumberRangeMax;
                    gradingIndexVM.LetterGrade = item.LetterGrade;
                    gradingIndexVM.GradePoint = item.GradePoint;
                    gradingIndexVM.Id = item.Id;
                    gradingIndexVMs.Add(gradingIndexVM);
                }
            }

            return View(gradingIndexVMs);
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(GradingTable obj)
        {
            if (ModelState.IsValid)
            {
                obj.CreatedAt = DateTime.Now;
                obj.CreatedBy = HttpContext.Session.GetString("UserId");
                obj.MACAddress = MACService.GetMAC();
                bool IsValidObj = ValidateGradingTableObject(obj);
                if (IsValidObj)
                {
                    bool isSaved = await _gradingTableManager.AddAsync(obj);
                    if (isSaved)
                    {
                        TempData["created"] = "Grading Row Created Successfully.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["error"] = "Failed to add new grading system.";
                }
            }
            return View(obj);
        }

        private bool ValidateGradingTableObject(GradingTable obj)
        {
            if (obj.NumberRangeMin > obj.NumberRangeMax)
            {
                return false;
            }
            return true;
        }
    }
}
