using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.App.ViewModels.Employees;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;


namespace SchoolManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly IEmployeeManager _employeeManager;
        private readonly IMapper _mapper;
        private readonly IGenderManager _genderManager;
        private readonly IReligionManager _religionManager;
        private readonly INationalityManager _nationalityManager;
        private readonly IEmpTypeManager _empTypeManager;
        private readonly IDesignationManager _designationManager;
        private readonly IDivisionManager _divisionManager;

        public EmployeesController(IWebHostEnvironment host,  IEmployeeManager employeeManager, IGenderManager genderManager, IReligionManager religionManager, IMapper mapper, INationalityManager nationalityManager, IEmpTypeManager empTypeManager, IDesignationManager designationManager, IDivisionManager divisionManager) 
        {
            _host = host;
            _employeeManager = employeeManager;
            _genderManager = genderManager;
            _mapper = mapper;
            _religionManager = religionManager;
            _nationalityManager = nationalityManager;
            _empTypeManager = empTypeManager;
            _designationManager = designationManager;
            _divisionManager = divisionManager;
        }

        
        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var empList = await _employeeManager.GetAllAsync();
            return View(empList);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myid = Convert.ToInt32(id);
            Employee employee = await _employeeManager.GetByIdAsync(myid);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            EmployeeCreateVM employee = new EmployeeCreateVM();
            employee.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name").ToList();
            employee.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name").ToList();
            employee.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name").ToList();
            employee.EmpTypeList = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name").ToList();
            employee.DesignationList = new SelectList(await _designationManager.GetAllAsync(), "Id", "DesignationName").ToList();
            employee.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();

            //ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            //ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            //ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            //ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            //ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            //ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status")] EmployeeCreateVM employeeVM, IFormFile empImage, IFormFile nidCard)
        {
            if (empImage != null)
            {
                string root = _host.WebRootPath;
                string folder = "~/Images/Employee";
                string fileExtension = Path.GetExtension(empImage.FileName);
                string fileName = "e_" + DateTime.Today.ToString("yyyy") + "_" + employeeVM.NIDNo;
                string pathCombine = Path.Combine(root, folder, fileName);

            }

            if (nidCard != null)
            {

            }

            var employee = _mapper.Map<Employee>(employeeVM);

            if (ModelState.IsValid)
            {
                

                bool isSaved =await _employeeManager.AddAsync(employee);
                if (isSaved)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            


            //ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "DesignationName",employee.DesignationId);
            //ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name");
            //ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Name");
            //ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Name");
            //ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            //ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            //ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            //ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Name");
            //ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Name");
            //ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Name");
            //ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Name");
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeManager.GetByIdAsync((int)id);
            if (employee == null)
            {
                return NotFound();
            }
            //ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "Id", employee.DesignationId);
            //ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name", employee.EmpTypeId);
            //ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Id", employee.GenderId);
            //ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Id", employee.NationalityId);
            //ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PermanentDistrictId);
            //ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PermanentDivisionId);
            //ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PermanentUpazilaId);
            //ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PresentDistrictId);
            //ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PresentDivisionId);
            //ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PresentUpazilaId);
            //ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Id", employee.ReligionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status")] Employee employee)
        {
            string msg = "";
            if (id != employee.Id)
            {

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employee.EditedAt = DateTime.Now;
                    employee.EditedBy = HttpContext.Session.GetString("UserId");
                    bool isUpdated =await _employeeManager.UpdateAsync(employee);

                    //bool isSaved =await _empRepository.Update(employee);
                    if (isUpdated)
                    {
                        msg = "Employee info edited";
                        ViewBag.msg = msg;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                msg = "Something wrong";
            }
            //ViewData["DesignationId"] = new SelectList(_context.Designation, "Id", "DesignationName", employee.DesignationId);
            //ViewData["EmpTypeId"] = new SelectList(_context.EmpType, "Id", "Name", employee.EmpTypeId);
            //ViewData["GenderId"] = new SelectList(_context.Gender, "Id", "Id", employee.GenderId);
            //ViewData["NationalityId"] = new SelectList(_context.Nationality, "Id", "Id", employee.NationalityId);
            //ViewData["PermanentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PermanentDistrictId);
            //ViewData["PermanentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PermanentDivisionId);
            //ViewData["PermanentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PermanentUpazilaId);
            //ViewData["PresentDistrictId"] = new SelectList(_context.District, "Id", "Id", employee.PresentDistrictId);
            //ViewData["PresentDivisionId"] = new SelectList(_context.Division, "Id", "Id", employee.PresentDivisionId);
            //ViewData["PresentUpazilaId"] = new SelectList(_context.Upazila, "Id", "Id", employee.PresentUpazilaId);
            //ViewData["ReligionId"] = new SelectList(_context.Religion, "Id", "Id", employee.ReligionId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int myId = Convert.ToInt32(id);
            var employee = await _employeeManager.GetByIdAsync(myId);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _employeeManager.GetByIdAsync(id);
            await _employeeManager.RemoveAsync(emp);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            var employee= _employeeManager.GetByIdAsync(id);
            if (employee!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
