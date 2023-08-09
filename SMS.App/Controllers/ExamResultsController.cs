using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem;
using SMS.App.ViewModels.ExamVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize]
    public class ExamResultsController : Controller
    {
        private readonly IExamResultManager _examResultManager;
        private readonly IAcademicExamManager _academicExamManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentManager _studentManager;
        private readonly IAcademicExamTypeManager _academicExamTypeManager;
        private readonly IAcademicClassManager _academicClassManager;
        public ExamResultsController(IExamResultManager examResultManager, IAcademicExamManager academicExamManager, UserManager<ApplicationUser> userManager, IStudentManager studentManager, IAcademicExamTypeManager academicExamTypeManager, IAcademicClassManager academicClassManager)
        {
            _examResultManager = examResultManager;
            _academicExamManager = academicExamManager;
            _userManager = userManager;
            _studentManager = studentManager;
            _academicExamTypeManager = academicExamTypeManager;
            _academicClassManager = academicClassManager;
        }
        // GET: ExamResultsController
        public async Task<ActionResult> Index()
        {
            List<ExamResultVM> exams = new();

            List<AcademicExam> existingExams = (List<AcademicExam>)await _academicExamManager.GetAllAsync();
            var user =await _userManager.GetUserAsync(User);

            ViewData["ExamType"] = new SelectList(await _academicExamTypeManager.GetAllAsync(),"Id", "ExamTypeName");
            ViewData["ClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
                
                
                //await _academicExamTypeManager.GetAllAsync();
            if (user.UserType == 's')
            {
                Student student = await _studentManager.GetByIdAsync(user.ReferenceId);
                existingExams = existingExams.Where(e => e.AcademicSectionId == student.AcademicSectionId &&
                    //e.AcademicSessionId == student.AcademicSessionId &&
                    e.AcademicSubject.AcademicClassId == student.AcademicClassId)
                    .ToList();
            }

            if (existingExams!=null)
            {
                foreach (AcademicExam academicExam in existingExams)
                {
                    
                    ExamResultVM exam = new();
                    exam.ExamId = academicExam.Id;
                    //exam.ExamName = academicExam.ExamName;
                    //exam.AcademicClass = academicExam.AcademicSubject.AcademicClass.Name;
                    //exam.TotalExaminee = academicExam.AcademicExamDetails.Count;
                    exams.Add(exam);
                }
            }
            return View(exams);
        }

        public async Task<ActionResult> SubjecttWiseResult()
        {
            GlobalUI.PageTitle = GlobalUI.SiteTitle = "Student-Wise Result";
            return View();
        }
        public async Task<ActionResult> ClassWiseResult()
        {
            GlobalUI.PageTitle = GlobalUI.SiteTitle = "Class-Wise Result";
            return View();
        }
        public async Task<ActionResult> StudentWiseResult()
        {
            GlobalUI.PageTitle = GlobalUI.SiteTitle = "Student-Wise Result";

            ViewData["ExamType"] = new SelectList(await _academicExamTypeManager.GetAllAsync(), "Id", "ExamTypeName");
            ViewData["ClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");


            return View();
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
