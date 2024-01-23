using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.ConfigureVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class ParamBusConfigController : Controller
    {
        private readonly IParamBusConfigManager _paramBusConfigManager;
        private readonly IMapper _mapper;
        public ParamBusConfigController(IParamBusConfigManager paramBusConfigManager, IMapper mapper)
        {
            _paramBusConfigManager = paramBusConfigManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            GlobalUI.PageTitle = "Configure Parameter";
            var configs = await _paramBusConfigManager.GetAllAsync();
            ParamBusConfigVM paramBusConfigVM = new ParamBusConfigVM();
            paramBusConfigVM.ParamBusConfigs = (IList<ParamBusConfig>)configs;
            return View(paramBusConfigVM);
        }
        public IActionResult UpSert(int? id = 0)
        {
            if (id!=null || id>0)
            {
                GlobalUI.PageTitle = "Update Configure Parameter";
            }
            else
            {
                GlobalUI.PageTitle = "Create Configure Parameter";
            }
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpSert(ParamBusConfig paramBusConfig)
        {
            if (paramBusConfig.Id>0)
            {
                GlobalUI.PageTitle = "Update Configure Parameter";
                ParamBusConfig existingParamBusConfig = await _paramBusConfigManager.GetByIdAsync(paramBusConfig.Id);
                try
                {
                    existingParamBusConfig.EditedBy = HttpContext.Session.GetString("UserId");
                    existingParamBusConfig.EditedAt = DateTime.Now;
                    existingParamBusConfig.MACAddress = MACService.GetMAC();
                    existingParamBusConfig.ParamValue = paramBusConfig.ParamValue;
                    existingParamBusConfig.ParamSL = paramBusConfig.ParamSL;
                    existingParamBusConfig.ConfigName = paramBusConfig.ConfigName;
                    existingParamBusConfig.Remarks = paramBusConfig.Remarks;
                    existingParamBusConfig.IsActive = paramBusConfig.IsActive;
                    bool isUpdated = await _paramBusConfigManager.UpdateAsync(existingParamBusConfig);
                    if (isUpdated)
                    {
                        TempData["created"] = "Configuration data updated Successfully";
                    }
                }
                catch (Exception)
                {

                    TempData["failed"] = "Exception! Fail to update";
                }
            }
            else
            {
                GlobalUI.PageTitle = "Create Configure Parameter";
                try
                {
                    if (ModelState.IsValid)
                    {
                        paramBusConfig.CreatedAt = DateTime.Now;
                        paramBusConfig.CreatedBy = HttpContext.Session.GetString("UserId");
                        paramBusConfig.EditedBy = HttpContext.Session.GetString("UserId");
                        paramBusConfig.EditedAt = DateTime.Now;
                        paramBusConfig.MACAddress = MACService.GetMAC();
                        bool isSaved = await _paramBusConfigManager.AddAsync(paramBusConfig);
                        if (isSaved)
                        {
                            TempData["created"] = "New Configuration data added Successfully";
                        }
                    }
                }
                catch (Exception)
                {
                    TempData["failed"] = "Exception! Fail to create";
                }
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
