using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.QuestionBank;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class QuestionBanksController : Controller
    {
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IQuestionManager _questionManager;
        private readonly IWebHostEnvironment _host;
        public QuestionBanksController(IAcademicClassManager academicClassManager, IQuestionManager questionManager, IWebHostEnvironment host)
        {
            _academicClassManager = academicClassManager;
            _questionManager = questionManager;
            _host = host;
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
        public async Task<JsonResult> CreateQuestion(QuestionVM model, IFormFile qImage, IFormCollection collection)
        {
            Question nQuestion = new Question();
            nQuestion.QuestionDetails = new List<QuestionDetails>();
            nQuestion.Uddipok = model.QCreateVM.Uddipok;
            if (model.QCreateVM.Image != null)
            {
                string questionImage = "";
                string root = _host.WebRootPath;
                if (string.IsNullOrEmpty(root))
                {
                    root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string folder = "Images/Question";
                string fileExtension = Path.GetExtension(qImage.FileName);
                questionImage ="q_"+model.QCreateVM.AcademicClassId+"_"+model.QCreateVM.AcademicSubjectId+"_"+"_"+model.QCreateVM.ChapterId+"_"+nQuestion.Id+ fileExtension;
                string pathCombine = Path.Combine(root, folder, questionImage);
                using var stream = new FileStream(pathCombine, FileMode.Create);
                await qImage.CopyToAsync(stream);                

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
