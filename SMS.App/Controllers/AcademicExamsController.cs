using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.ExamVM;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize]
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
        private readonly IStudentManager _studentManager;
        private readonly IAcademicExamDetailsManager _academicExamDetailsManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AcademicExamsController(IAcademicExamManager examManager, IAcademicSessionManager sessionManager, IAcademicClassManager classManager,IAcademicExamTypeManager examTypeManager,IAcademicSubjectManager academicSubjectManager, IEmployeeManager employeeManager,IMapper mapper,IAcademicSectionManager academicSectionManager, IStudentManager studentManager, IAcademicExamDetailsManager academicExamDetailsManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _examManager = examManager;
            _sessionManager = sessionManager;
            _classManager = classManager;
            _examTypeManager = examTypeManager;
            _academicSubjectManager = academicSubjectManager;
            _employeeManager = employeeManager;
            _mapper  = mapper;
            _academicSectionManager = academicSectionManager;
            _studentManager = studentManager;
            _academicExamDetailsManager = academicExamDetailsManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: AcademicExamsController
        [Authorize(Roles ="Admin, Teacher, SuperAdmin")]
        public async Task<ActionResult> Index()
        {
            var exams = await _examManager.GetAllAsync();

            bool isAdminUser = false;
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                if (item.Contains("Admin") || item.Contains("SuperAdmin"))
                {
                    isAdminUser = true;
                    break;
                }
            }
            if (isAdminUser!=true)
            {
                exams = exams.Where(m => m.EmployeeId == user.ReferenceId).ToList();
            }

            return View(exams.OrderByDescending(m => m.MonthId).ThenBy(m => m.AcademicSubject.AcademicClassId).ThenBy(m=>m.AcademicSectionId));
        }

        // GET: AcademicExamsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var exam = await _examManager.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdminUser = false;
            foreach (var item in roles)
            {
                if (item.Contains("Admin") || item.Contains("SuperAdmin"))
                {
                    isAdminUser = true;
                    break;
                }
            }
            if (user.UserType == 'e')
            {
                if (user.ReferenceId != exam.EmployeeId)
                {
                    if (isAdminUser == false)
                    {
                        return RedirectToAction("AccessDenied", "Accounts");
                    }
                }
            }

            ExamDetailsVM examDetailsVM = new ExamDetailsVM();
            examDetailsVM.ExamId = exam.Id;
            examDetailsVM.AcademicClassId = exam.AcademicSubject.AcademicClassId;
            examDetailsVM.AcademicExamDetails = examDetailsVM.AcademicExamDetails;
            examDetailsVM.TotalMarks = exam.TotalMarks;
            examDetailsVM.ExamMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(exam.MonthId);
            examDetailsVM.ExamName = exam.ExamName;
            examDetailsVM.Teacher = exam.Employee.EmployeeName;
            examDetailsVM.AcademicSubjectName = exam.AcademicSubject.SubjectName;
            examDetailsVM.AcademicClassName = exam.AcademicSubject.AcademicClass.Name;
            examDetailsVM.AcademicSessionName = exam.AcademicSession.Name;
            examDetailsVM.AcademicExamDetails = exam.AcademicExamDetails;
            examDetailsVM.AcademicSectionId = exam.AcademicSectionId;
            examDetailsVM.AcademicSubjectCode = exam.AcademicSubject.SubjectCode.ToString();
            if (exam.AcademicSection!=null)
            {
                examDetailsVM.AcademicSectionName = exam.AcademicSection.Name;
            }
            examDetailsVM.IsActive = exam.IsActive;
            
            return View(examDetailsVM);
        }

        // GET: AcademicExamsController/Create
        [Authorize(Roles = "SuperAdmin, Admin")]
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult> Create(AcademicExamVM academicExamVM)
        {
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            academicExamVM.AcademicSessionList = new SelectList(await _sessionManager.GetAllAsync(), "Id", "Name").ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(), "Id", "Name").ToList();
            academicExamVM.AcademicExamTypeList = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName").ToList();
            academicExamVM.AcademicSubjectList = new SelectList(await _academicSubjectManager.GetSubjectsByClassIdAsync(academicExamVM.AcademicClassId), "Id", "SubjectName").ToList();
            academicExamVM.AcademicSectionList = new SelectList(await _academicSectionManager.GetAllByClassWithSessionId(academicExamVM.AcademicClassId,academicExamVM.AcademicSessionId), "Id", "Name", academicExamVM.AcademicSectionId).ToList();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status == true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName), "Id", "EmployeeName").ToList();

            bool examIsExist = false;
            

            try
            {
                List<AcademicExam> existingExams = (List<AcademicExam>)await _examManager.GetAllAsync();

                examIsExist = existingExams.Any(
                    e => e.AcademicSubjectId == academicExamVM.AcademicSubjectId &&
                    e.AcademicSectionId == academicExamVM.AcademicSectionId &&
                    e.AcademicExamTypeId == academicExamVM.AcademicExamTypeId &&
                    e.AcademicSessionId == academicExamVM.AcademicSessionId &&
                    e.MonthId == academicExamVM.MonthId);
                if (examIsExist)
                {
                    TempData["error"] = "Sorry! This Exam is already exist.";
                    return View(academicExamVM);
                }
                AcademicExam academicExam = _mapper.Map<AcademicExam>(academicExamVM);



                academicExam.CreatedAt = DateTime.Now;
                academicExam.CreatedBy = HttpContext.Session.GetString("UserId");
                academicExam.MACAddress = MACService.GetMAC();
                AcademicSubject academicSubject = await _academicSubjectManager.GetByIdAsync(academicExam.AcademicSubjectId);    

                List<AcademicExamDetail>  objListAcademicExamDetails = new List<AcademicExamDetail>();
                List<Student> students = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(academicExam.AcademicSessionId, academicExamVM.AcademicClassId);
                if(academicSubject.ReligionId!=null || academicSubject.ReligionId>0)
                {
                    students = students.Where(s => s.AcademicSectionId == academicExam.AcademicSectionId && s.ReligionId == academicSubject.ReligionId).ToList();
                }
                if (students.Count<=0)
                {
                    TempData["error"] = "There has no any student available for this subject";
                    return View(academicExamVM);
                }
                foreach (var student in students)
                {
                    if (student.Status == false)
                    {
                        continue;
                    }
                    else
                    {
                        AcademicExamDetail academicExamDetail = new AcademicExamDetail();
                        academicExamDetail.AcademicExamId = academicExam.Id;
                        academicExamDetail.StudentId = student.Id;
                        academicExamDetail.MACAddress = MACService.GetMAC();
                        academicExamDetail.CreatedAt = DateTime.Now;
                        academicExamDetail.CreatedBy = HttpContext.Session.GetString("UserId");
                        academicExamDetail.ObtainMark = 0;
                        academicExamDetail.Status = true;

                        objListAcademicExamDetails.Add(academicExamDetail);
                    }
                }

                academicExam.AcademicExamDetails = objListAcademicExamDetails;


                bool isSaved = await _examManager.AddAsync(academicExam);
                if (isSaved)
                {
                    TempData["created"] = "New Exam Created Successfully";
                    academicExamVM.AcademicSectionId = academicExamVM.AcademicSectionId == null ? 0 : academicExamVM.AcademicSectionId;
                    //List<Student> students = await _studentManager
                    //.GetStudentsByClassSessionSectionAsync(academicExamVM.AcademicSessionId, academicExamVM.AcademicClassId, (int)academicExamVM.AcademicSectionId);
                    //if (academicSubject.ReligionId != null || academicSubject.ReligionId > 0)
                    //{
                        
                    //}
                    //    foreach (var st in students)
                    //{

                    //    if (st.Status == false)
                    //    {
                    //        continue;
                    //    }
                    //    AcademicExamDetail academicExamDetail = new()
                    //    {
                    //        AcademicExamId = academicExam.Id,
                    //        ObtainMark = 0,
                    //        StudentId = st.Id,
                    //        Status = true,
                    //        CreatedAt = DateTime.Now,
                    //        CreatedBy = HttpContext.Session.GetString("UserId"),
                    //        MACAddress = MACService.GetMAC()
                    //    };
                    //    await _academicExamDetailsManager.AddAsync(academicExamDetail);
                    //}

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
        [Authorize(Roles = "SuperAdmin, Admin")]
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
        [Authorize(Roles = "SuperAdmin, Admin")]
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
        public async Task<ActionResult> Delete(int id)
        {
            AcademicExam academicExam = await _examManager.GetByIdAsync(id);
            return View(academicExam);
        }

        // POST: AcademicExamsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult> Delete(int id, AcademicExam objAcademicExam)
        {
            if (id != objAcademicExam.Id)
            {
                TempData["error"]= "Not Found";                
                return View(objAcademicExam);
            }
            try
            {
                AcademicExam existingAcademicExam = await _examManager.GetByIdAsync(id);
                if (existingAcademicExam == null)
                {
                    TempData["error"] = "Not Found";
                    return View(objAcademicExam);
                }

                bool isDeleted = await _examManager.RemoveAsync(existingAcademicExam);
                if (isDeleted)
                {
                    TempData["deleted"] = "Exam has been successfully removed";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = "Fail to delete.";
                return View(objAcademicExam);
            }
            catch
            {
                return View(objAcademicExam);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExmaMarkSubmit(ExamDetailsVM examDetailVM)
        {
            List<AcademicExamDetail> academicExamDetail = new ();
            academicExamDetail= examDetailVM.AcademicExamDetails;
            foreach (AcademicExamDetail item in academicExamDetail)
            {
                item.MACAddress = MACService.GetMAC();
                item.EditedAt = DateTime.Now;
                item.EditedBy = HttpContext.Session.GetString("UserId");

                await _academicExamDetailsManager.UpdateAsync(item);
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> AdmitCard()
        {
            ViewData["ExamType"] = new SelectList(await _examTypeManager.GetAllAsync(), "Id", "ExamTypeName");
            ViewData["AcademicClass"] = new SelectList(await _classManager.GetAllAsync(), "Id", "Name");

            return View();
        }
        
        [HttpPost]
        public async Task<JsonResult> UnlockExam(int exId)
        {
            string msg = string.Empty;
            var existingExam = await _examManager.GetByIdAsync(exId);
            if (existingExam != null)
            {
                existingExam.IsActive = false;
                bool isUpdated = await _examManager.UpdateAsync(existingExam);
                if (isUpdated)
                {
                    msg = "Exam is Unlocked Successfully";
                }
                else
                {
                    msg = "Unloacked faild";
                }
                return Json(new { exId = exId,msg=msg });
            }
            return Json(new { msg = "Exam not found!" });
        }
        [HttpPost]
        public async Task<ActionResult> LockExam(int exId)
        {
            string msg = string.Empty;
            var existingExam = await _examManager.GetByIdAsync(exId);
            if (existingExam != null)
            {
                existingExam.IsActive = true;
                bool isUpdated = await _examManager.UpdateAsync(existingExam);
                if (isUpdated)
                {
                    msg = "Exam is Locked Successfully";
                }
                else
                {
                    msg = "Loacked faild";
                }
                return Json(new { exId = exId, msg = msg });
            }
            return Json(new { msg = "Exam not found!" });
        }
    }
}
