using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SMS.App.Utilities.MACIPServices;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class OffDayTypeController : Controller
    {
        private readonly IOffDayTypeManager _offDayTypeManager;
        public OffDayTypeController(IOffDayTypeManager offDayTypeManager)
        {
            _offDayTypeManager = offDayTypeManager;
        }
        // GET: OffDayTypeController
        public async Task<ActionResult> Index()
        {
            var result = await _offDayTypeManager.GetAllAsync();
            return View(result);
        }

        // GET: OffDayTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await _offDayTypeManager.GetByIdAsync(id);
            return View(result);
        }

        // GET: OffDayTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OffDayTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OffDayType offDayType)
        {
            try
            {
                var newObject = new OffDayType();
                newObject.OffDayTypeName = offDayType.OffDayTypeName;
                newObject.Remarks = offDayType.Remarks;
                newObject.MACAddress = MACService.GetMAC();
                newObject.CreatedAt = DateTime.Now;
                newObject.CreatedBy = HttpContext.Session.GetString("UserId");
                bool isAdded = await _offDayTypeManager.AddAsync(newObject);
                if (isAdded)
                {
                    TempData["created"] = "New Off Day Type created";                    
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View(offDayType);
            }
            return View(offDayType);
        }

        // GET: OffDayTypeController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            OffDayType offDayType = await _offDayTypeManager.GetByIdAsync(id);
            return View(offDayType);
        }

        // POST: OffDayTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (collection["Id"] != id)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                OffDayType offDayType = await _offDayTypeManager.GetByIdAsync(id);
                if (offDayType.OffDayTypeName != collection["OffDayTypeName"]) 
                {
                    offDayType.OffDayTypeName = collection["OffDayTyupeName"];
                    TempData["updated"] = "Off Day Type Name Updated Successfully";
                }
                else
                {
                    TempData[""] = "Change not found";
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OffDayTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OffDayTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
