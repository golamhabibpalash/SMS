using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SMS.App.Utilities.MACIPServices;
using SMS.App.ViewModels.AcademicVM;
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
        private readonly IAcademicExamGroupManager _examGroupManager;
        public AcademicExamsController(IAcademicExamManager examManager, IAcademicSessionManager sessionManager, IAcademicClassManager classManager,IAcademicExamTypeManager examTypeManager,IAcademicSubjectManager academicSubjectManager, IEmployeeManager employeeManager,IMapper mapper,IAcademicSectionManager academicSectionManager, IStudentManager studentManager, IAcademicExamDetailsManager academicExamDetailsManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAcademicExamGroupManager academicExamGroupManager)
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
            _examGroupManager = academicExamGroupManager;
        }

        // GET: AcademicExamsController
        [Authorize(Roles ="Admin, Teacher, SuperAdmin")]
        public async Task<ActionResult> Index()
        {
            ViewModels.AcademicVM.AcademicExamVM academicExamVM = new ViewModels.AcademicVM.AcademicExamVM();
            AcademicSession currentSession = await _sessionManager.GetCurrentAcademicSession();
            academicExamVM.AcademicExamGroupList = new SelectList(await _examGroupManager.GetAllAsync(currentSession.Id),"Id", "ExamGroupName").ToList();
            academicExamVM.AcademicClassList = new SelectList(await _classManager.GetAllAsync(), "Id", "Name").ToList();
            List<Employee> emps = (List<Employee>)await _employeeManager.GetAllAsync();
            academicExamVM.TeacherList = new SelectList(emps.Where(e => e.Status == true).OrderBy(e => e.JoiningDate).ThenBy(e => e.EmployeeName), "Id", "EmployeeName").ToList();
            var exams = await _examManager.GetAllAsync();
            if (exams!=null)
            {
                academicExamVM.AcademicExams = (List<AcademicExam>)exams;
            }

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

            return View(academicExamVM);
        }

        // GET: AcademicExamsController/Details/5
        public async Task<ActionResult> Details(int id)
        {

            var exam = await _examManager.GetByIdAsync(id);
            if (exam==null)
            {
                TempData["error"]="Data not found";
                return RedirectToAction("index");
            }
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
            ViewModels.AcademicVM.AcademicExamVM academicExamVM = new ViewModels.AcademicVM.AcademicExamVM();
            academicExamVM.AcademicExamGroup = exam.AcademicExamGroup;
            academicExamVM.AcademicExamDetails = exam.AcademicExamDetails;
            academicExamVM.AcademicClass = exam.AcademicClass;
            academicExamVM.AcademicSection = exam.AcademicSection;
            academicExamVM.AcademicSubject = exam.AcademicSubject;
            academicExamVM.Employee = exam.Employee;
            academicExamVM.TotalMarks = exam.TotalMarks;
            
            return View(exam);
        }

        
        // POST: AcademicExamsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult> Create(List<AcademicExam> AcademicExam)
        {
            int success = 0;
            int failed = 0;
            var existingExams = await _examManager.GetAllAsync();
            try
            {
                if (AcademicExam.Count>0)
                {
                    foreach (AcademicExam exam in AcademicExam)
                    {
                        var isExist = existingExams.FirstOrDefault(s => s.AcademicExamGroupId == exam.AcademicExamGroupId && s.AcademicClassId == exam.AcademicClassId && s.AcademicSubjectId == exam.AcademicSubjectId);
                        if (isExist!=null)
                        {
                            failed++;
                            continue;
                        }
                        AcademicSubject academicSubject = await _academicSubjectManager.GetByIdAsync(exam.AcademicSubjectId);
                        exam.CreatedAt = DateTime.Now;
                        exam.CreatedBy = HttpContext.Session.GetString("UserId");
                        exam.MACAddress = MACService.GetMAC();
                        bool isSaved = await _examManager.AddAsync(exam);
                        if (isSaved)
                        {
                            success++;
                            AcademicExamGroup academicExamGroup = await _examGroupManager.GetByIdAsync(exam.AcademicExamGroupId);
                            var students = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(academicExamGroup.AcademicSessionId, exam.AcademicClassId);
                            foreach (Student student in students.Where(s => s.Status=true))
                            {
                                if (exam.AcademicSectionId!=null || exam.AcademicSectionId>0)
                                {
                                    if (student.AcademicSectionId != exam.AcademicSectionId)
                                    {
                                        continue;
                                    }
                                }
                                if (academicSubject.ReligionId!=null || academicSubject.ReligionId>=0)
                                {
                                    if (student.ReligionId != academicSubject.ReligionId)
                                    {
                                        continue;
                                    }
                                }
                                AcademicExamDetail academicExamDetail = new AcademicExamDetail();
                                academicExamDetail.AcademicExamId = exam.Id;
                                academicExamDetail.ObtainMark = 0;
                                academicExamDetail.StudentId = student.Id;
                                academicExamDetail.Status = true;
                                academicExamDetail.CreatedAt = DateTime.Now;
                                academicExamDetail.CreatedBy = HttpContext.Session.GetString("UserId");
                                academicExamDetail.MACAddress = MACService.GetMAC();
                                await _academicExamDetailsManager.AddAsync(academicExamDetail);
                            }
                        }
                        else
                        {
                            failed++;
                        }
                    }
                    TempData["created"] = "Success:"+success + " added & Failed: "+failed;
                }
                else
                {
                    TempData["created"] = "No data found to add";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Exception: "+ex.Message;
            }
            return RedirectToAction("index");
        }


        // POST: AcademicExamsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult> Edit(int id, AcademicExam academicExam, ViewModels.ExamVM.AcademicExamVM academicExamVM)
        {

            //Minimum checking
            if (id!=academicExam.Id)
            {
                TempData["error"] = "Data Id mismatched.";
                return RedirectToAction("index");
            }
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Failed! Error:"+ModelState.ErrorCount+" Please fillup the form properly.";
                return RedirectToAction("index");
            }
            //Checking, is already exist!
            var allExams = await _examManager.GetAllAsync();
            AcademicExam existingExam = allExams.FirstOrDefault(e => e.Id != academicExam.Id && e.AcademicExamGroupId == academicExam.AcademicExamGroupId && e.AcademicClassId == academicExam.AcademicClassId && e.AcademicSubjectId == academicExam.AcademicSubjectId);
            if (existingExam != null)
            {
                TempData["error"] = "Exam is already exist in this group";
                return RedirectToAction("index");
            }
            //Checking is it same data!
            AcademicExam exam = await _examManager.GetByIdAsync(academicExam.Id);
            if (exam.AcademicExamGroupId == academicExam.AcademicExamGroupId && 
                exam.AcademicClassId == academicExam.AcademicClassId && 
                exam.AcademicSectionId==academicExam.AcademicSectionId && 
                exam.AcademicSubjectId == academicExam.AcademicSubjectId &&
                exam.EmployeeId == academicExam.EmployeeId && 
                exam.TotalMarks == academicExam.TotalMarks &&
                exam.Status == academicExam.Status)
            {
                TempData["error"] = "oh ho! Same data, Nothing to change!";
                return RedirectToAction("index");
            }

            try
            {
                academicExam.EditedAt = DateTime.Now;
                academicExam.EditedBy = HttpContext.Session.GetString("UserId");
                academicExam.MACAddress = MACService.GetMAC();
                bool isUpdate = await _examManager.UpdateAsync(academicExam);
                if (isUpdate)
                {
                    AcademicSubject academicSubject = await _academicSubjectManager.GetByIdAsync(academicExam.AcademicSubjectId);
                    if (exam.AcademicSubjectId != academicExam.AcademicSubjectId)
                    {
                        var examDetails = await _academicExamDetailsManager.GetByExamIdAsync(exam.Id);
                        if (examDetails != null)
                        {
                            foreach (var eDetail in examDetails)
                            {
                                await _academicExamDetailsManager.RemoveAsync(eDetail);
                            }
                        }
                        AcademicExamGroup academicExamGroup = await _examGroupManager.GetByIdAsync(academicExam.AcademicExamGroupId);
                        var students = await _studentManager.GetStudentsByClassIdAndSessionIdAsync(academicExamGroup.AcademicSessionId, academicExam.AcademicClassId);
                        foreach (Student student in students.Where(s => s.Status = true))
                        {
                            if (academicExam.AcademicSectionId != null || academicExam.AcademicSectionId > 0)
                            {
                                if (student.AcademicSectionId != academicExam.AcademicSectionId)
                                {
                                    continue;
                                }
                            }
                            if (academicSubject.ReligionId != null || academicSubject.ReligionId >= 0)
                            {
                                if (student.ReligionId != academicSubject.ReligionId)
                                {
                                    continue;
                                }
                            }
                            AcademicExamDetail academicExamDetail = new AcademicExamDetail();
                            academicExamDetail.AcademicExamId = exam.Id;
                            academicExamDetail.ObtainMark = 0;
                            academicExamDetail.StudentId = student.Id;
                            academicExamDetail.Status = true;
                            academicExamDetail.CreatedAt = DateTime.Now;
                            academicExamDetail.CreatedBy = HttpContext.Session.GetString("UserId");
                            academicExamDetail.MACAddress = MACService.GetMAC();
                            await _academicExamDetailsManager.AddAsync(academicExamDetail);
                        }
                    }

                    TempData["created"] = "success! Data updated successfully";
                    return RedirectToAction("index");
                }
                else
                {
                    TempData["error"] = "Failed! Something wrong. Please try again later.";
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Execption: "+ex.Message;
                return RedirectToAction("index");
            }
        }

        // GET: AcademicExamsController/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            AcademicExam academicExam = await _examManager.GetByIdAsync(id);
            try
            {
                bool isRemoved = await _examManager.RemoveAsync(academicExam);
                if (isRemoved)
                {
                    TempData["created"] = "Data deleted successfully.";
                }
                else
                {
                    TempData["error"] = "Failed to delete";
                }
                return Json("ok");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Exception:"+ex.Message;
            }
            return Json("");
        }

        // POST: AcademicExamsController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin, Admin")]
        //public async Task<ActionResult> Delete(int id, AcademicExam objAcademicExam)
        //{
        //    if (id != objAcademicExam.Id)
        //    {
        //        TempData["error"]= "Not Found";                
        //        return View(objAcademicExam);
        //    }
        //    try
        //    {
        //        AcademicExam existingAcademicExam = await _examManager.GetByIdAsync(id);
        //        if (existingAcademicExam == null)
        //        {
        //            TempData["error"] = "Not Found";
        //            return View(objAcademicExam);
        //        }

        //        bool isDeleted = await _examManager.RemoveAsync(existingAcademicExam);
        //        if (isDeleted)
        //        {
        //            TempData["deleted"] = "Exam has been successfully removed";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        TempData["error"] = "Fail to delete.";
        //        return View(objAcademicExam);
        //    }
        //    catch
        //    {
        //        return View(objAcademicExam);
        //    }
        //}


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
                //existingExam.IsActive = false;
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
                //existingExam.IsActive = true;
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
