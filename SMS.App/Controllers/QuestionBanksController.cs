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

        [HttpPost]
        public async Task<JsonResult> CreateQuestion(QuestionVM model)
        {
            Question nQuestion = new Question();
            nQuestion.Uddipok = model.QCreateVM.Uddipok;
            if (model.QCreateVM.Image != null)
            {
                nQuestion.Image = model.QCreateVM.Image.ToString();
                nQuestion.ImagePosition = model.QCreateVM.ImagePosition;
            }
            nQuestion.ChapterId = model.QCreateVM.ChapterId;

            List<QuestionDetails> nQuestionDetails = new List<QuestionDetails>();

            QuestionDetails questionDetails1 = new QuestionDetails() {QuestionId = nQuestion.Id,QuestionText =model.QCreateVM.Question_1, CreatedAt = DateTime.Now, CreatedBy = HttpContext.Session.GetString("UserId"), MACAddress = MACService.GetMAC() };
            nQuestionDetails.Add(questionDetails1);

            QuestionDetails questionDetails2 = new QuestionDetails() { QuestionId = nQuestion.Id, QuestionText = model.QCreateVM.Question_2, CreatedAt = DateTime.Now, CreatedBy = HttpContext.Session.GetString("UserId"), MACAddress = MACService.GetMAC() };
            nQuestionDetails.Add(questionDetails2);

            QuestionDetails questionDetails3 = new QuestionDetails() { QuestionId = nQuestion.Id, QuestionText = model.QCreateVM.Question_3, CreatedAt = DateTime.Now, CreatedBy = HttpContext.Session.GetString("UserId"), MACAddress = MACService.GetMAC() };
            nQuestionDetails.Add(questionDetails3);

            QuestionDetails questionDetails4 = new QuestionDetails() { QuestionId = nQuestion.Id, QuestionText = model.QCreateVM.Question_4, CreatedAt = DateTime.Now, CreatedBy = HttpContext.Session.GetString("UserId"), MACAddress = MACService.GetMAC() };
            nQuestionDetails.Add(questionDetails4);

            nQuestion.QuestionDetails=nQuestionDetails;
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
                        return Json(nQuestion);
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
