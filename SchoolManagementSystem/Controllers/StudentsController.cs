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
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repository;
using SchoolManagementSystem.ViewModels;

namespace SchoolManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _host;
        private readonly StudentRepository _studentRepository;
        private readonly AcademicClassRepository _academicClassRepository;

        public StudentsController(StudentRepository stRepository, ApplicationDbContext context, AcademicClassRepository academicClassRepository, IWebHostEnvironment host)
        {
            _studentRepository = stRepository;
            _academicClassRepository = academicClassRepository;
            _context = context;
            _host = host;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var student = await _studentRepository.GetAllAsync();
            return View(student);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int myId = Convert.ToInt32(id);
            var student =await _studentRepository.GetByIdAsync(myId);
            if (student == null)
            {
                return NotFound();
            }
            var stuPayments = await _context.StudentPayment
                .Include(sp => sp.StudentPaymentDetails)
                .ThenInclude(sp => sp.StudentFeeHead)
                .Where(sp => sp.StudentId == id).ToListAsync();
            StudentDetailsVM sd = new();
            sd.StudentPayments = stuPayments;
            sd.Student = student;
            ViewBag.districts = _context.District.ToList();
            ViewBag.Upazila = _context.Upazila.ToList();

            return View(sd);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["AcademicClassId"] = new SelectList(_academicClassRepository.GetAll(), "Id", "Name");
            ViewData["AcademicSessionId"] = new SelectList(_context.AcademicSession, "Id", "Name");
            ViewData["AcademicSectionId"] = new SelectList(_context.Set<AcademicSection>(), "Id", "Name");
            ViewData["BloodGroupId"] = new SelectList(_context.Set<BloodGroup>(), "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "Id", "Name");
            ViewData["NationalityId"] = new SelectList(_context.Set<Nationality>(), "Id", "Name");
            ViewData["ReligionId"] = new SelectList(_context.Set<Religion>(), "Id", "Name");
            ViewData["DivisionList"] = new SelectList(_context.Set<Division>().OrderBy(d => d.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClassRoll,FatherName,MotherName,AdmissionDate,Email,PhoneNo,Photo,DOB,ReligionId,GenderId,BloodGroupId,NationalityId,PresentAddressArea,PresentAddressPO,PresentUpazilaId,PresentDistrictId,PresentDivisiontId,PermanentAddressArea,PermanentAddressPO,PermanentUpazilaId,PermanentDistrictId,PermanentDivisiontId,AcademicSessionId,AcademicClassId,AcademicSectionId,PreviousSchool,Status,CreatedBy,CreatedAt,EditedBy,EditedAt,GuardianPhone")] Student student, IFormFile sPhoto)
        {
            if (ModelState.IsValid)
            {
                if (sPhoto!=null)
                {
                    string fileExt = Path.GetExtension(sPhoto.FileName);
                    string root = _host.WebRootPath;
                    string folder = "Images/Student/";
                    string year = _context.AcademicSession.FirstOrDefault(s => s.Id == student.AcademicSessionId).Name.Substring(0,4);
                    string fileName = "S_" + year + "_" + student.ClassRoll + fileExt;
                    string pathCombine = Path.Combine(root, folder, fileName);
                    using (var stream = new FileStream(pathCombine, FileMode.Create))
                    {
                        await sPhoto.CopyToAsync(stream);
                    }
                    student.Photo = fileName;
                }
                student.CreatedBy = HttpContext.Session.GetString("UserId");
                student.CreatedAt = DateTime.Now;
                bool saveStudent = await _studentRepository.SaveAsync(student);
                if (saveStudent==true)
                {
                    TempData["create"] = "Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["AcademicSessionId"] = new SelectList(_context.Set<AcademicSession>(), "Id", "Name", student.AcademicSessionId);
            ViewData["classList"] = new SelectList(_context.Set<AcademicClass>(), "Id", "Name", student.AcademicClassId);
            ViewData["sectionList"] = new SelectList(_context.Set<AcademicSection>(), "Id", "Name", student.AcademicSectionId);
            ViewData["BloodGroupId"] = new SelectList(_context.Set<BloodGroup>(), "Id", "Name", student.BloodGroupId);
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "Id", "Name", student.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Set<Nationality>(), "Id", "Name", student.NationalityId);
            ViewData["ReligionId"] = new SelectList(_context.Set<Religion>(), "Id", "Name", student.ReligionId);
            ViewData["DivisionList"] = new SelectList(_context.Set<Division>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["DistrictList"] = new SelectList(_context.Set<District>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["UpList"] = new SelectList(_context.Set<Upazila>().OrderBy(d => d.Name), "Id", "Name");
            return View(student);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = (int)(id);
            var student = await _studentRepository.GetByIdAsync(myId);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["AcademicSessionId"] = new SelectList(_context.Set<AcademicSession>(), "Id", "Name", student.AcademicSessionId);
            ViewData["classList"] = new SelectList(_context.Set<AcademicClass>(), "Id", "Name", student.AcademicClassId);
            ViewData["sectionList"] = new SelectList(_context.Set<AcademicSection>(), "Id", "Name", student.AcademicSectionId);
            ViewData["BloodGroupId"] = new SelectList(_context.Set<BloodGroup>(), "Id", "Name", student.BloodGroupId);
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "Id", "Name", student.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Set<Nationality>(), "Id", "Name", student.NationalityId);
            ViewData["ReligionId"] = new SelectList(_context.Set<Religion>(), "Id", "Name", student.ReligionId);
            ViewData["DivisionList"] = new SelectList(_context.Set<Division>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["DistrictList"] = new SelectList(_context.Set<District>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["UpList"] = new SelectList(_context.Set<Upazila>().OrderBy(d => d.Name), "Id", "Name");
            return View(student);
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
                        string year = _context.AcademicSession.FirstOrDefault(s => s.Id == student.AcademicSessionId).Name.Substring(0, 4);
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

                    isUpdated = await _studentRepository.UpdateAsync(student);

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

            ViewData["AcademicSessionId"] = new SelectList(_context.Set<AcademicSession>(), "Id", "Name", student.AcademicSessionId);
            ViewData["classList"] = new SelectList(_context.Set<AcademicClass>(), "Id", "Name", student.AcademicClassId);
            ViewData["sectionList"] = new SelectList(_context.Set<AcademicSection>(), "Id", "Name", student.AcademicSectionId);
            ViewData["BloodGroupId"] = new SelectList(_context.Set<BloodGroup>(), "Id", "Name", student.BloodGroupId);
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "Id", "Name", student.GenderId);
            ViewData["NationalityId"] = new SelectList(_context.Set<Nationality>(), "Id", "Name", student.NationalityId);
            ViewData["ReligionId"] = new SelectList(_context.Set<Religion>(), "Id", "Name", student.ReligionId);
            ViewData["DivisionList"] = new SelectList(_context.Set<Division>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["DistrictList"] = new SelectList(_context.Set<District>().OrderBy(d => d.Name), "Id", "Name");
            ViewData["UpList"] = new SelectList(_context.Set<Upazila>().OrderBy(d => d.Name), "Id", "Name");
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = (int)(id);
            var student = await _studentRepository.GetByIdAsync(myId);

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
            var student = await _studentRepository.GetByIdAsync(id);
            bool isSaved = await _studentRepository.DeleteAsync(student);
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
            return _context.Student.Any((System.Linq.Expressions.Expression<Func<Student, bool>>)(e => e.Id == id));
        }

        public async Task<JsonResult> GetClassList(int id)
        {
            var classList =await _academicClassRepository.GetAllBySessionIdAsync(id);
            return Json(classList);
        }
        public async Task<JsonResult> GetSectionList(int id)
        {
            var sectionList =await _context.AcademicSection
                .Where(s => s.AcademicClassId == id)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return Json(sectionList);
        }
        public async Task<JsonResult> GetDistrictList(int id)
        {
            var districtList =await _context.District.
                Where(s => s.DivisionId == id)
                .OrderBy(d => d.Name)
                .ToListAsync();
            return Json(districtList);
        }
        public async Task<JsonResult> GetUpazilaList(int id)
        {
            var upazilaList =await _context.Upazila
                .Where(s => s.DistrictId == id)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Json(upazilaList);
        }
    }
}