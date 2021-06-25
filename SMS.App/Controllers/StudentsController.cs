using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.DB;
using SMS.Entities;
using SMS.App.ViewModels.Students;
using Repositories;
using SMS.DAL.Repositories;
using SMS.BLL.Contracts;
using AutoMapper;

namespace SchoolManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IStudentManager _studentManager;
        private readonly IAcademicClassManager _academicClassManager;
        private readonly IMapper _mapper;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly IStudentPaymentManager _studentPaymentManager;
        private readonly IDivisionManager _divisionManager;
        private readonly IDistrictManager _districtManager;
        private readonly IUpazilaManager _upazilaManager;
        private readonly IAcademicSectionManager _academicSectionManager;
        private readonly IBloodGroupManager _bloodGroupManager;
        private readonly INationalityManager _nationalityManager;
        private readonly IGenderManager _genderManager;
        private readonly IReligionManager _religionManager;


        public StudentsController(IStudentManager studentManager, IAcademicClassManager academicClassManager, IWebHostEnvironment host, IMapper mapper, IAcademicSessionManager academicSessionManager, IStudentPaymentManager studentPaymentManager, IDistrictManager districtManager, IUpazilaManager upazilaManager, IAcademicSectionManager academicSectionManager, IBloodGroupManager bloodGroupManager, IDivisionManager divisionManager, INationalityManager nationalityManager, IGenderManager genderManager, IReligionManager religionManager)
        {
            _academicClassManager = academicClassManager;
            _host = host;
            _studentManager = studentManager;
            _academicSessionManager = academicSessionManager;
            _mapper = mapper;
            _studentPaymentManager = studentPaymentManager;
            _districtManager = districtManager;
            _upazilaManager = upazilaManager;
            _academicSectionManager = academicSectionManager;
            _bloodGroupManager = bloodGroupManager;
            _divisionManager = divisionManager;
            _nationalityManager = nationalityManager;
            _genderManager = genderManager;
            _religionManager = religionManager;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var student = await _studentManager.GetAllAsync();
            var studentList = _mapper.Map<IEnumerable<StudentListVM>>(student);
            return View(studentList);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student =await _studentManager.GetByIdAsync((int)id);
            if (student == null)
            {
                return NotFound();
            }
            var stuPayments = await _studentPaymentManager.GetAllByStudentIdAsync((int)id);

            StudentDetailsVM sd = new();
            sd.StudentPayments = stuPayments;
            sd.Student = student;
            ViewBag.districts = await _districtManager.GetAllAsync();
            ViewBag.Upazila = await _upazilaManager.GetAllAsync();

            return View(sd);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            StudentCreateVM student = new();
            student.AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name").ToList();
            student.AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name").ToList();
            student.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name").ToList();
            student.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name").ToList();
            student.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name").ToList();
            student.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name").ToList();
            student.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClassRoll,FatherName,MotherName,AdmissionDate,Email,PhoneNo,Photo,DOB,ReligionId,GenderId,BloodGroupId,NationalityId,PresentAddressArea,PresentAddressPO,PresentUpazilaId,PresentDistrictId,PresentDivisiontId,PermanentAddressArea,PermanentAddressPO,PermanentUpazilaId,PermanentDistrictId,PermanentDivisiontId,AcademicSessionId,AcademicClassId,AcademicSectionId,PreviousSchool,Status,CreatedBy,CreatedAt,EditedBy,EditedAt,GuardianPhone")] StudentCreateVM newStudent, IFormFile sPhoto)
        {
            if (ModelState.IsValid)
            {
                if (sPhoto!=null)
                {
                    string fileExt = Path.GetExtension(sPhoto.FileName);
                    string root = _host.WebRootPath;
                    string folder = "Images/Student/";
                    string sessionYear =(await _academicSessionManager.GetByIdAsync(newStudent.AcademicSessionId)).ToString();
                    string year =sessionYear.Substring(0,4);
                    string fileName = "S_" + year + "_" + newStudent.ClassRoll + fileExt;
                    string pathCombine = Path.Combine(root, folder, fileName);
                    using (var stream = new FileStream(pathCombine, FileMode.Create))
                    {
                        await sPhoto.CopyToAsync(stream);
                    }
                    newStudent.Photo = fileName;
                }
                newStudent.CreatedBy = HttpContext.Session.GetString("UserId");
                newStudent.CreatedAt = DateTime.Now;

                var student = _mapper.Map<Student>(newStudent);

                bool saveStudent = await _studentManager.AddAsync(student);
                if (saveStudent==true)
                {
                    TempData["create"] = "Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            

            newStudent.AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name",newStudent.AcademicSessionId).ToList();
            newStudent.AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name",newStudent.AcademicClassId).ToList();
            newStudent.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name", newStudent.BloodGroupId).ToList();
            newStudent.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name",newStudent.GenderId).ToList();
            newStudent.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name",newStudent.NationalityId).ToList();
            newStudent.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name",newStudent.ReligionId).ToList();
            newStudent.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();



            return View(newStudent);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = (int)(id);
            var student = await _studentManager.GetByIdAsync(myId);
            if (student == null)
            {
                return NotFound();
            }
            var newStudent = _mapper.Map<StudentCreateVM>(student);
            newStudent.AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", newStudent.AcademicSessionId).ToList();
            newStudent.AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", newStudent.AcademicClassId).ToList();
            newStudent.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name", newStudent.BloodGroupId).ToList();
            newStudent.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name", newStudent.GenderId).ToList();
            newStudent.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name", newStudent.NationalityId).ToList();
            newStudent.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name", newStudent.ReligionId).ToList();
            newStudent.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();

            
            return View(newStudent);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClassRoll,FatherName,MotherName,AdmissionDate,Email,PhoneNo,Photo,DOB,ReligionId,GenderId,BloodGroupId,NationalityId,PresentAddressArea,PresentAddressPO,PresentUpazilaId,PresentDistrictId,PresentDivisiontId,PermanentAddressArea,PermanentAddressPO,PermanentUpazilaId,PermanentDistrictId,PermanentDivisiontId,AcademicSessionId,AcademicClassId,AcademicSectionId,PreviousSchool,Status,CreatedBy,CreatedAt,EditedBy,EditedAt,GuardianPhone")] Student student, IFormFile sPhoto)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool isUpdated = false;
                try
                {
                    if (sPhoto != null)
                    {
                        string fileExt = Path.GetExtension(sPhoto.FileName);
                        string root = _host.WebRootPath;
                        string folder = "Images/Student/";
                        string sessionYear = (await _academicSessionManager.GetByIdAsync(student.AcademicSessionId)).ToString();
                        string year = sessionYear.Substring(0, 4);
                        string fileName = "S_" + year + "_" + student.ClassRoll + fileExt;
                        string pathCombine = Path.Combine(root, folder, fileName);
                        using (var stream = new FileStream(pathCombine, FileMode.Create))
                        {
                            await sPhoto.CopyToAsync(stream);
                        }
                        student.Photo = fileName;
                    }


                    student.EditedBy = HttpContext.Session.GetString("UserId");
                    student.EditedAt = DateTime.Now;

                    isUpdated = await _studentManager.UpdateAsync(student);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (isUpdated ==true)
                {
                    TempData["edit"] = "Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            var newStudent = _mapper.Map<StudentCreateVM>(student);
            newStudent.AcademicSessionList = new SelectList(await _academicSessionManager.GetAllAsync(), "Id", "Name", newStudent.AcademicSessionId).ToList();
            newStudent.AcademicClassList = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name", newStudent.AcademicClassId).ToList();
            newStudent.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name", newStudent.BloodGroupId).ToList();
            newStudent.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name", newStudent.GenderId).ToList();
            newStudent.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name", newStudent.NationalityId).ToList();
            newStudent.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name", newStudent.ReligionId).ToList();
            newStudent.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();

            return View(newStudent);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = (int)(id);
            var student = await _studentManager.GetByIdAsync(myId);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentManager.GetByIdAsync(id);
            bool isSaved = await _studentManager.RemoveAsync(student);
            if (isSaved==true)
            {
                TempData["delete"] = "Deleted Successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Delete", new { id });
            }
            
        }

        private bool StudentExists(int id)
        {
            var student = _studentManager.GetByIdAsync(id);
            if (student!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public async Task<JsonResult> GetClassList()
        //{
        //    var classList =await _academicClassManager.GetAllAsync();
        //    return Json(classList);
        //}
        //public async Task<JsonResult> GetSectionList(int id)
        //{
        //    var sectionList =await _context.AcademicSection
        //        .Where(s => s.AcademicClassId == id)
        //        .OrderBy(s => s.Name)
        //        .ToListAsync();
        //    return Json(sectionList);
        //}
        //public async Task<JsonResult> GetDistrictList(int id)
        //{
        //    var districtList =await _context.District.
        //        Where(s => s.DivisionId == id)
        //        .OrderBy(d => d.Name)
        //        .ToListAsync();
        //    return Json(districtList);
        //}
        //public async Task<JsonResult> GetUpazilaList(int id)
        //{
        //    var upazilaList =await _context.Upazila
        //        .Where(s => s.DistrictId == id)
        //        .OrderBy(s => s.Name)
        //        .ToListAsync();

        //    return Json(upazilaList);
        //}
    }
}