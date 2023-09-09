using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using SchoolManagementSystem;
using SMS.App.ViewModels.AttendanceVM;
using SMS.App.ViewModels.ExamResult;
using SMS.App.ViewModels.ExamVM;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ExamResultsController : Controller
    {
        private readonly IExamResultManager _examResultManager;
        private readonly IAcademicExamManager _academicExamManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentManager _studentManager;
        private readonly IAcademicExamTypeManager _academicExamTypeManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IAcademicExamGroupManager _academicExamGroupManager;
        private readonly IAcademicSessionManager _sessionManager;
        private readonly IGradingTableManager _gradingTableManager;
        private readonly IInstituteManager _instituteManager;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly IOffDayManager _OffDayManager;
        public ExamResultsController(IExamResultManager examResultManager, IAcademicExamManager academicExamManager, UserManager<ApplicationUser> userManager, IStudentManager studentManager, IAcademicExamTypeManager academicExamTypeManager, IAcademicClassManager academicClassManager, IAcademicExamGroupManager academicExamGroupManager, IAcademicSessionManager sessionManager, IGradingTableManager gradingTableManager, IInstituteManager instituteManager, IAttendanceMachineManager attendanceMachineManager, IOffDayManager offDayManager)
        {
            _examResultManager = examResultManager;
            _academicExamManager = academicExamManager;
            _userManager = userManager;
            _studentManager = studentManager;
            _academicExamTypeManager = academicExamTypeManager;
            _academicClassManager = academicClassManager;
            _academicExamGroupManager = academicExamGroupManager;
            _sessionManager = sessionManager;
            _gradingTableManager = gradingTableManager;
            _instituteManager = instituteManager;
            _attendanceMachineManager = attendanceMachineManager;
            _OffDayManager = offDayManager;
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

        public async Task<ActionResult> Result()
        {
            GlobalUI.PageTitle = "Academic Results";

            ViewData["ExamGroupList"] = new SelectList(await _academicExamGroupManager.GetAllAsync(), "Id", "ExamGroupName");
            ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Result(string resultType,int examGroupId, int classId)
        {
            return View();
        }
        public async Task<ActionResult> SubjecttWiseResult()
        {
            GlobalUI.PageTitle = GlobalUI.SiteTitle = "Student-Wise Result";
            return View();
        }
        public async Task<ActionResult> ClassWiseResult()
        {
            GlobalUI.PageTitle = GlobalUI.SiteTitle = "Class-Wise Result";
            ViewData["ExamGroupList"] = new SelectList(await _academicExamGroupManager.GetAllAsync(), "Id", "ExamGroupName");
            ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");

            ViewBag.IsLoading = false;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ClassWiseResult(string resultType, int examGroupId, int classId)
        {
            GlobalUI.PageTitle = "Class-Wise Result";
            ViewData["ExamGroupList"] = new SelectList(await _academicExamGroupManager.GetAllAsync(), "Id", "ExamGroupName",examGroupId);
            ViewData["AcademicClassList"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name",classId);

            var examList = await _academicExamManager.GetByClassIdExamGroupId(examGroupId, classId);
            if (examList == null || examList.Count <= 0)
            {
                TempData["failed"] = "Exam Not Found";
                ViewBag.IsLoading = true;
                return View();
            }
            var currentSession = await _sessionManager.GetCurrentAcademicSession();
            var students = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(currentSession.Id, classId);
            List<ExaminationResultVM> examinationResultVMs = new List<ExaminationResultVM>();
            var institute = await _instituteManager.GetFirstOrDefaultAsync();
            var examGroup = await _academicExamGroupManager.GetByIdAsync(examGroupId);
            var academicClass = await _academicClassManager.GetByIdAsync(classId);

            ViewBag.InstituteName = institute.Name;
            ViewBag.logo = institute.Logo;
            ViewBag.ExamGroupName = examGroup.ExamGroupName;
            ViewBag.academicClassName = academicClass.Name;
            var newManupulateResult = examList.SelectMany(s => s.AcademicExamDetails).GroupBy(s => new { s.AcademicExamId, s.StudentId }).Select(group => new
            {
                examId = group.Key.AcademicExamId,
                studentId = group.Key.StudentId,
                obtainMark = group.Sum(m => m.ObtainMark)
            }).ToList();

            int monthDays = ViewBag.daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, examGroup.ExamMonthId);

            DateTime firstDateOfMonth = new(DateTime.Now.Year, examGroup.ExamMonthId, 1);
            DateTime lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);

            string StartDate = firstDateOfMonth.ToString("yyyy-MM-dd");
            string EndDate = lastDateOfMonth.ToString("yyyy-MM-dd");

            var attendanceList = await _attendanceMachineManager.GetAttendanceByDateRangeAsync(StartDate,EndDate);
            List<DateTime> monthlyHolidays = await _OffDayManager.GetMonthlyHolidaysAsync(firstDateOfMonth.ToString("MMyyyy"));

            foreach (var student in students.Where(s => s.Status == true).OrderBy(s => s.ClassRoll))
            {
                ExaminationResultVM examinationResultVM = new ExaminationResultVM();
                examinationResultVM.ClassRoll = student.ClassRoll;
                examinationResultVM.StudentName = student.Name;
                double obtainPoint = 0.00;
                int subCount = 0;
                int totalFail = 0;
                foreach (var exam in examList)
                {
                    ExaminationResultDetailsVMs exDetail = new ExaminationResultDetailsVMs();

                    exDetail.SubjectName = exam.AcademicSubject.SubjectName;
                    exDetail.IsReligion = exam.AcademicSubject.ReligionId!=null?true:false;
                    exDetail.ObtainMarks = exam.AcademicExamDetails.Where(s => s.StudentId == student.Id && s.AcademicExamId == exam.Id).Sum(s => s.ObtainMark);
                    exDetail.ObtainPoint = await GetGradePointByNumber((exDetail.ObtainMarks * 100)/exam.TotalMarks);
                    exDetail.ObtainGrade = await GetGradeByPoint(exDetail.ObtainPoint);
                    exDetail.ExamMarks = exam.TotalMarks;
                    exDetail.HighestObtainMark = exam.AcademicExamDetails
                        .Where(s => s.AcademicExamId == exam.Id)
                        .OrderByDescending(s => s.ObtainMark)
                        .Select(s => s.ObtainMark).Max();

                    examinationResultVM.ExaminationResultDetailsVMs.Add(exDetail);
                    subCount++;
                    
                    if (exDetail.ObtainPoint <= 0)
                    {
                        totalFail++;
                    }
                    obtainPoint += exDetail.ObtainPoint;
                    
                }
                examinationResultVM.FailSubCount = totalFail;
                examinationResultVM.CGPA = examinationResultVM.FailSubCount > 0 ? 0.00 : (obtainPoint / subCount);
                examinationResultVM.TotalMarks = newManupulateResult.Where(s => s.studentId == student.Id).Sum(s => s.obtainMark);
                examinationResultVM.Grade = await GetGradeByPoint(examinationResultVM.CGPA);
                examinationResultVM.GradeComment = await GetGradeComments(examinationResultVM.CGPA);
                var myAttendances = attendanceList.Where(t => t.CardNo == student.ClassRoll.ToString().PadLeft(8, '0')).ToList();
                IDictionary<int, bool> daysPresents = new Dictionary<int, bool>();
                for (int i = 1; i <= monthDays; i++)
                {
                    var currentDate = firstDateOfMonth.AddDays(i - 1).ToString("ddMMyyyy");
                    var attended = myAttendances.Where(a => a.PunchDatetime.ToString("ddMMyyyy") == currentDate).Any();
                    if (attended)
                    {
                        daysPresents.Add(i, true);
                    }
                    else
                    {
                        daysPresents.Add(i, false);
                    }
                }
                int total = daysPresents.Where(m => m.Value == true).Count();
                examinationResultVM.AttendancePercent = (total * 100) / (monthDays - monthlyHolidays.Count);
                examinationResultVM.PreviousMonthRank = 10;
                examinationResultVMs.Add(examinationResultVM);
            }
            var rankedResults = examinationResultVMs
                .OrderByDescending(result => result.CGPA)
                .ThenBy(result => result.FailSubCount)
                .ThenByDescending(result => result.TotalMarks)
                .ThenByDescending(result => result.AttendancePercent)
                .Select((result, index) => new ExaminationResultVM
                {
                    TotalMarks = result.TotalMarks,
                    CGPA = result.CGPA,
                    FailSubCount = result.FailSubCount,
                    Rank = index + 1,
                    ClassRoll = result.ClassRoll
                })
                .ToList();
            foreach (var result in examinationResultVMs)
            {
                result.Rank = rankedResults.Where(s => s.ClassRoll == result.ClassRoll).Select(s => s.Rank).FirstOrDefault();
            }
            ViewBag.IsLoading = true;
            return View(examinationResultVMs);
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

        private async Task<double> GetGradePointByNumber(double number)
        {
            double point = 0.00;
            try
            {
                var gradings = await _gradingTableManager.GetAllAsync();

                var result = from g in gradings
                             where number >= g.NumberRangeMin && number <= g.NumberRangeMax
                             select g.GradePoint;

                point = Math.Round(Convert.ToDouble(result.FirstOrDefault()),2);
            }
            catch (Exception)
            {
                throw;
            }

            return point;
        }
        private async Task<string> GetGradeByPoint(double number)
        {
            string grade = string.Empty;
            try
            {
                var gradings = await _gradingTableManager.GetAllAsync();

                var result = (from g in gradings
                             where number >= Convert.ToDouble(g.GradePoint)
                             select g.LetterGrade).ToList().Take(1);


                grade = result.FirstOrDefault().ToString();
            }
            catch (Exception)
            {

                throw;
            }
            return grade;
        }
        private async Task<string> GetGradeComments(double number)
        {
            string gradeComments = string.Empty;
            try
            {
                var gradings = await _gradingTableManager.GetAllAsync();

                var result = (from g in gradings
                              where number >= Convert.ToDouble(g.GradePoint)
                              select g.gradeComments).ToList().Take(1);

                if (result!=null)
                {
                    gradeComments = result.FirstOrDefault().ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return gradeComments;
        }
    }
}
