using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.App.ViewModels.ExamVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class ExamResultsController : Controller
    {
        private readonly IExamResultManager _examResultManager;
        private readonly IAcademicExamManager _academicExamManager;
        public ExamResultsController(IExamResultManager examResultManager, IAcademicExamManager academicExamManager)
        {
            _examResultManager = examResultManager;
            _academicExamManager = academicExamManager;

        }
        // GET: ExamResultsController
        public async Task<ActionResult> Index()
        {
            List<ExamResultVM> exams = new();
            List<AcademicExam> existingExams = (List<AcademicExam>)await _academicExamManager.GetAllAsync();
            if (existingExams!=null)
            {
                foreach (AcademicExam academicExam in existingExams)
                {
                    ExamResultVM exam = new();
                    exam.ExamId = academicExam.Id;
                    exam.ExamName = academicExam.ExamName;
                    exam.AcademicClass = academicExam.AcademicSubject.AcademicClass.Name;
                    exam.TotalExaminee = academicExam.AcademicExamDetails.Count;
                    exams.Add(exam);
                }
            }
            return View(exams);
        }

        // GET: ExamResultsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExamResultsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExamResultsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: ExamResultsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExamResultsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: ExamResultsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExamResultsController/Delete/5
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
