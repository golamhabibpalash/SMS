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
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class QuestionBanksController : Controller
    {
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAcademicSubjectManager _academicSubjectManager;
        private readonly IQuestionManager _questionManager;
        private readonly IWebHostEnvironment _host;
        public QuestionBanksController(IAcademicClassManager academicClassManager, IQuestionManager questionManager, IAcademicSubjectManager academicSubjectManager, IWebHostEnvironment host)
        {
            _academicSubjectManager = academicSubjectManager;
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
        public async Task<JsonResult> CreateQuestion(QuestionVM model, IList<IFormFile> files, IFormFile qImage, IFormCollection collection)
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
            if (model.QCreateVM.QuestionDetails.Count<=0)
            {
                return Json(new {msg="Questions did not found, try again."});
            }
            var existingSubject = await _academicSubjectManager.GetByIdAsync(model.QCreateVM.AcademicSubjectId);            
            List<int> qMarks = existingSubject.QuestionFormat.QFormat.Split(',').Select(int.Parse).ToList();
            int qSl = 0;
            foreach (var qd in model.QCreateVM.QuestionDetails)
            {
                qd.QuestionId = nQuestion.Id;
                qd.MACAddress = MACService.GetMAC();
                qd.CreatedBy = HttpContext.Session.GetString("UserId");
                qd.CreatedAt = DateTime.Now;
                qd.QMark = qMarks.ElementAt(qSl);
                nQuestion.QuestionDetails.Add(qd);
                qSl++;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var existingQuestion = await _questionManager.GetByIdAsync(id);

            QuestionEditVM questionEditVM = new QuestionEditVM();
            questionEditVM.Question = existingQuestion;
            var aClass = await _academicClassManager.GetByIdAsync(existingQuestion.Chapter.AcademicSubject.AcademicClassId);
            questionEditVM.AcademicClassId = aClass.Id;
            var aSubject = await _academicSubjectManager.GetByIdAsync(existingQuestion.Chapter.AcademicSubjectId);
            questionEditVM.AcademicSubjectId = aSubject.Id;
            
            List<AcademicSubject> academicSubjects = new List<AcademicSubject>();
            academicSubjects.Add(aSubject);

            List<Chapter> chapters = new List<Chapter>();
            chapters.Add(existingQuestion.Chapter);

            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name",questionEditVM.AcademicClassId);
            ViewData["AcademicSubjectId"] = new SelectList(academicSubjects, "Id", "SubjectName", questionEditVM.AcademicSubjectId);
            ViewData["ChapterId"] = new SelectList(chapters, "Id", "ChapterName", questionEditVM.Question.ChapterId);

            return View(questionEditVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionEditVM question)
        {
            
            return View();
        }
    }
}
