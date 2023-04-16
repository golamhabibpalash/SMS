using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.ExamVM;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    public class AcademicExamsController : Controller
    {
        private readonly IAcademicExamManager _examManager;
        private readonly IAcademicSessionManager _sessionManager;
        private readonly IAcademicClassManager _classManager;
        private readonly IAcademicExamTypeManager _examTypeManager;
        private readonly IAcademicSubjectManager _academicSubjectManager;
        private readonly IEmployeeManager _employeeManager;
        private readonly IMapper _mapper;
        private readonly IAcademicSectionManager _academicSectionManager;
        public AcademicExamsController(IAcademicExamManager examManager, IAcademicSessionManager sessionManager, IAcademicClassManager classManager,IAcademicExamTypeManager examTypeManager,IAcademicSubjectManager academicSubjectManager, IEmployeeManager employeeManager,IMapper mapper,IAcademicSectionManager academicSectionManager )
        {
            _examManager = examManager;
            _sessionManager = sessionManager;
            _classManager = classManager;
            _examTypeManager = examTypeManager;
            _academicSubjectManager = academicSubjectManager;
            _employeeManager = employeeManager;
            _mapper  = mapper;
            _academicSectionManager = academicSectionManager;
        }

        // GET: AcademicExamsController
        public async Task<ActionResult> Index()
        {
            var exams = await _examManager.GetAllAsync();

            return View(exams);
        }

        // GET: AcademicExamsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var exam = await _examManager.GetByIdAsync(id);
            return View(exam);
        }

        // GET: AcademicExamsController/Create
        public async Task<ActionResult> Create()
        {
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            AcademicExamVM academicExamVM = new();
            academicExamVM.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(),"Id","Name").ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(),"Id","Name").ToList();
            academicExamVM.AcademicExamTypeList = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName").ToList();
            academicExamVM.AcademicSubjectList = new SelectList(await _academicSubjectManager.GetAllAsync(),"Id", "SubjectName").ToList();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status==true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName),"Id", "EmployeeName").ToList();
            return View(academicExamVM);
        }

        // POST: AcademicExamsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AcademicExamVM academicExamVM)
        {
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            academicExamVM.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(), "Id", "Name").ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(), "Id", "Name").ToList();
            academicExamVM.AcademicExamTypeList = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName").ToList();
            academicExamVM.AcademicSubjectList = new SelectList(await _academicSubjectManager.GetAllAsync(), "Id", "SubjectName").ToList();
            academicExamVM.AcademicSectionList = new SelectList(await _academicSectionManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicSectionId).ToList();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status == true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName), "Id", "EmployeeName").ToList();

            try
            {
                AcademicExam academicExam = _mapper.Map<AcademicExam>(academicExamVM);
                academicExam.CreatedAt = DateTime.Now;
                academicExam.CreatedBy = HttpContext.Session.GetString("UserId");
                academicExam.MACAddress = MACService.GetMAC();
                bool isSaved = await _examManager.AddAsync(academicExam);
                if (isSaved)
                {
                    TempData["created"] = "New Exam Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View(academicExamVM);
            }
            return View(academicExamVM);
        }

        // GET: AcademicExamsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            AcademicExam aExam = await _examManager.GetByIdAsync(id);
            
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            AcademicExamVM academicExamVM = _mapper.Map<AcademicExamVM>(aExam);
            academicExamVM.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicSessionId).ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicClassId).ToList();
            academicExamVM.AcademicExamTypeList = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName", academicExamVM.AcademicExamTypeId).ToList();
            academicExamVM.AcademicSubjectList = new SelectList(await _academicSubjectManager.GetAllAsync(), "Id", "SubjectName", academicExamVM.AcademicSubjectId).ToList();
            academicExamVM.AcademicSectionList = new SelectList(await _academicSectionManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicSectionId).ToList();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status == true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName), "Id", "EmployeeName").ToList();
            return View(academicExamVM);
        }

        // POST: AcademicExamsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AcademicExamVM academicExamVM)
        {
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            academicExamVM.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicSessionId).ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicClassId).ToList();
            academicExamVM.AcademicExamTypeList = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName", academicExamVM.AcademicExamTypeId).ToList();
            academicExamVM.AcademicSubjectList = new SelectList(await _academicSubjectManager.GetAllAsync(), "Id", "SubjectName", academicExamVM.AcademicSubjectId).ToList();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status == true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName), "Id", "EmployeeName").ToList();
            academicExamVM.AcademicSectionList = new SelectList(await _academicSectionManager.GetAllAsync(), "Id", "Name", academicExamVM.AcademicSectionId).ToList();
            if (id!=academicExamVM.Id)
            {
                return View(academicExamVM);
            }
            try
            {
                AcademicExam aExam = _mapper.Map<AcademicExam>(academicExamVM);
                aExam.EditedAt = DateTime.Now;
                aExam.EditedBy = HttpContext.Session.GetString("UserId");
                aExam.MACAddress = MACService.GetMAC();
                bool isUpdated = await _examManager.UpdateAsync(aExam);
                if (isUpdated)
                {
                    TempData["updated"] = "Exam Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["failed"] = "Failed to Update";
                return View(academicExamVM);
            }
            catch
            {
                return View();
            }
        }

        // GET: AcademicExamsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcademicExamsController/Delete/5
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
