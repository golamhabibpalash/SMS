using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.QuestionBank;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class QuestionBanksController : Controller
    {
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IQuestionManager _questionManager;
        public QuestionBanksController(IAcademicClassManager academicClassManager, IQuestionManager questionManager)
        {
            _academicClassManager = academicClassManager;
            _questionManager = questionManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            return View();
        }

        public async Task<IActionResult> AllQuestion() 
        {
            var questionBanks = await _questionManager.GetAllAsync();
            return View(questionBanks);
        }

        [HttpPost]
        public async Task<JsonResult> CreateQuestion(QuestionVM model)
        {
            Question nQuestion = new Question();
            nQuestion.QuestionDetails = new List<QuestionDetails>();
            nQuestion.Uddipok = model.QCreateVM.Uddipok;
            if (model.QCreateVM.Image != null)
            {
                nQuestion.Image = model.QCreateVM.Image.ToString();
                nQuestion.ImagePosition = model.QCreateVM.ImagePosition;
            }
            nQuestion.ChapterId = model.QCreateVM.ChapterId;
            
            if (model.QCreateVM.QuestionDetails.Count==4)
            {
                foreach (var qd in model.QCreateVM.QuestionDetails)
                {
                    qd.QuestionId = nQuestion.Id;
                    qd.MACAddress = MACService.GetMAC();
                    qd.CreatedBy = HttpContext.Session.GetString("UserId");
                    qd.CreatedAt = DateTime.Now;
                    nQuestion.QuestionDetails.Add(qd);
                }
            }
            else
            {
                return Json("");
            }

            nQuestion.CreatedAt = DateTime.Now;
            nQuestion.CreatedBy = HttpContext.Session.GetString("UserId");
            nQuestion.MACAddress = MACService.GetMAC();

            if (ModelState.IsValid)
            {
                try
                {
                   bool isSaved = await _questionManager.AddAsync(nQuestion);
                    if (isSaved)
                    {
                        return Json(new {newQuestion=nQuestion, msg = "New Question added successfully."});
                    }
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
            return Json("");
        }

    }
}
