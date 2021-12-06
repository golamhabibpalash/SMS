using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.App.ViewModels.Employees;
using SMS.BLL.Contracts;
using SMS.DB;
using SMS.Entities;


namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
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
        private readonly IBloodGroupManager _bloodGroupManager;
        private readonly IUpazilaManager _upazilaManager;
        private readonly IDistrictManager _districtManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(IWebHostEnvironment host,  IEmployeeManager employeeManager, IGenderManager genderManager, IReligionManager religionManager, IMapper mapper, INationalityManager nationalityManager, IEmpTypeManager empTypeManager, IDesignationManager designationManager, IDivisionManager divisionManager, IDistrictManager districtManager, IUpazilaManager upazilaManager, IBloodGroupManager bloodGroupManager, UserManager<ApplicationUser> userManager) 
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
            _bloodGroupManager = bloodGroupManager;
            _districtManager = districtManager;
            _upazilaManager = upazilaManager;
            _userManager = userManager;

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
            var employeeDetailsVM = _mapper.Map<EmployeeDetailsVM>(employee);

            if (employeeDetailsVM == null)
            {
                return NotFound();
            }

            return View(employeeDetailsVM);
        }

        // GET: Employees/Create
        public async Task<IActionResult> Create()
        {
            EmployeeCreateVM employee = new ();
            employee.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name").ToList();
            employee.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name").ToList();
            employee.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name").ToList();
            employee.EmpTypeList = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name").ToList();
            employee.DesignationList = new SelectList(await _designationManager.GetAllAsync(), "Id", "DesignationName").ToList();
            employee.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();
            employee.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name").ToList();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status,BloodGroupId")] EmployeeCreateVM employeeVM, IFormFile empImage, IFormFile nidCard)
        {
            var employee1 = employeeVM;
            var bloodGroupList = await _bloodGroupManager.GetAllAsync();
            employee1.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name", employee1.GenderId).ToList();
            employee1.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name", employee1.ReligionId).ToList();
            employee1.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name", employee1.NationalityId).ToList();
            employee1.EmpTypeList = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name", employee1.EmpTypeId).ToList();
            employee1.DesignationList = new SelectList(await _designationManager.GetAllAsync(), "Id", "DesignationName", employee1.DesignationId).ToList();
            employee1.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name", employee1.DivisionList).ToList();
            employee1.BloodGroupList = new SelectList(bloodGroupList.OrderBy(b => b.Name), "Id", "Name", employee1.BloodGroupId).ToList();


            string empPhoto = "";
            string nidPhoto = "";
            bool isPhoneNoExist = false;
            bool isEmailExist = false;
            bool isNIDExist = false;
            var employees = await _employeeManager.GetAllAsync();

            var phoneDuplicate = employees.FirstOrDefault(e => e.Phone.ToString() == employeeVM.Phone.ToString());
            isPhoneNoExist = phoneDuplicate != null ? true : false;

            var emailDuplicate = employees.FirstOrDefault(e => e.Email == employeeVM.Email);
            isEmailExist = emailDuplicate != null ? true : false;

            var NIDDuplicate = employees.FirstOrDefault(e => e.NIDNo == employeeVM.NIDNo);
            isNIDExist = NIDDuplicate != null ? true : false;
            if (isPhoneNoExist == true || isEmailExist == true || isNIDExist == true)
            {
                if (isPhoneNoExist == true)
                {
                    ViewBag.msg = "Provided phone number is already exist";
                }
                if (isEmailExist == true)
                {
                    ViewBag.msg = "Provided email is already exist";
                }
                if (isNIDExist == true)
                {
                    ViewBag.msg = "Provided NID is already exist";
                }
                return View(employee1);
            }
            

            var employee = _mapper.Map<Employee>(employeeVM);

            if (ModelState.IsValid)
            {

                if (empImage != null)
                {
                    string root = _host.WebRootPath;
                    if (string.IsNullOrEmpty(root))
                    {
                        root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }
                    string folder = "Images/Employee/photo";
                    string fileExtension = Path.GetExtension(empImage.FileName);
                    empPhoto = "e_" + DateTime.Today.ToString("yyyy") + "_" + employeeVM.NIDNo + fileExtension;
                    string pathCombine = Path.Combine(root, folder, empPhoto);
                    using var stream = new FileStream(pathCombine, FileMode.Create);
                    await empImage.CopyToAsync(stream);
                }

                if (nidCard != null)
                {
                    string root = _host.WebRootPath;
                    string folder = "Images/Employee/NID";

                    string fileExtension = Path.GetExtension(nidCard.FileName);
                    nidPhoto = "e_" + employeeVM.NIDNo + fileExtension;
                    string pathCombine = Path.Combine(root, folder, nidPhoto);
                    using var stream = new FileStream(pathCombine, FileMode.Create);
                    await nidCard.CopyToAsync(stream);
                }

                employee.CreatedBy = HttpContext.Session.GetString("UserId");
                employee.CreatedAt = DateTime.Now;
                employee.Image = empPhoto;
                employee.NIDCard = nidPhoto;
                bool isSaved =await _employeeManager.AddAsync(employee);
                if (isSaved)
                {
                    TempData["saved"] = "Saved Successful";
                    var user = await _userManager.FindByEmailAsync(employee.Email);
                    if (user==null)
                    {
                        ApplicationUser identityUser = new ApplicationUser() { 
                            UserName = employee.Email,
                            NormalizedUserName= employee.EmployeeName,
                            Email = employee.Email,
                            NormalizedEmail = employee.Email,
                            ReferenceId = employee.Id,
                            UserType = 'e',
                            EmailConfirmed = true
                        };
                        var result = await _userManager.CreateAsync(identityUser, employee.Email + "A1");
                        if (result.Succeeded)
                        {
                            TempData["confirmationText"] = "User Created but not confirmed";
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return View(employee1);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeManager.GetByIdAsync((int)id);


            var employee1 = _mapper.Map<EmployeeEditVM>(employee);
            employee1.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name", employee.GenderId).ToList();
            employee1.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name", employee.ReligionId).ToList();
            employee1.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name", employee.NationalityId).ToList();
            employee1.EmpTypeList = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name", employee.EmpTypeId).ToList();
            employee1.DesignationList = new SelectList(await _designationManager.GetAllAsync(), "Id", "DesignationName", employee.DesignationId).ToList();
            employee1.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();
            employee1.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name", employee.BloodGroupId).ToList();

            ViewData["PermanentDistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", employee.PermanentDistrictId);
            ViewData["PermanentUpazilaId"] = new SelectList(await _upazilaManager.GetAllAsync(), "Id", "Name", employee.PermanentUpazilaId);
            ViewData["PresentDistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", employee.PresentDistrictId);
            ViewData["PresentUpazilaId"] = new SelectList(await _upazilaManager.GetAllAsync(), "Id", "Name", employee.PresentUpazilaId);


            if (employee1 == null)
            {
                return NotFound();
            }
            TempData["Image"] = employee.Image;
            TempData["NIDCard"] = employee.NIDCard;

            return View(employee1);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeName,DOB,Image,GenderId,ReligionId,NationalityId,NIDNo,NIDCard,Phone,Email,Nominee,NomineePhone,EmpTypeId,DesignationId,JoiningDate,PresentAddress,PresentUpazilaId,PresentDistrictId,PresentDivisionId,PermanentAddress,PermanentUpazilaId,PermanentDistrictId,PermanentDivisionId,CreatedBy,CreatedAt,EditedBy,EditedAt,Status,BloodGroupId")] EmployeeEditVM employeeVM, IFormFile Image, IFormFile NIDCard)
        {
            string msg = "";
            string empPhoto = "";
            string nidPhoto = "";
            if (id != employeeVM.Id)
            {
                return NotFound();
            }
            //var existEmployee = await _employeeManager.GetByIdAsync(employeeVM.Id);
            if (Image != null)
            {
                string root = _host.WebRootPath;
                if (string.IsNullOrEmpty(root))
                {
                    root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                string folder = "Images/Employee/photo";
                string fileExtension = Path.GetExtension(Image.FileName);
                empPhoto = "e_" + DateTime.Today.ToString("yyyy") + "_" + employeeVM.NIDNo + fileExtension;
                string pathCombine = Path.Combine(root, folder, empPhoto);
                using (var stream = new FileStream(pathCombine, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                employeeVM.Image = empPhoto;
            }
            else
            {
                employeeVM.Image = TempData["Image"].ToString();
            }

            if (NIDCard != null)
            {
                string root = _host.WebRootPath;
                string folder = "Images/Employee/NID";

                string fileExtension = Path.GetExtension(NIDCard.FileName);
                nidPhoto = "e_" + employeeVM.NIDNo + fileExtension;
                string pathCombine = Path.Combine(root, folder, nidPhoto);
                using (var stream = new FileStream(pathCombine, FileMode.Create))
                {
                    await NIDCard.CopyToAsync(stream);
                }
                employeeVM.NIDCard = nidPhoto;
            }
            else
            {
                employeeVM.NIDCard = TempData["NIDCard"].ToString();
            }


            var employee = _mapper.Map<Employee>(employeeVM);
            if (ModelState.IsValid)
            {
                try
                {
                    employee.EditedAt = DateTime.Now;
                    employee.EditedBy = HttpContext.Session.GetString("UserId");

                    bool isUpdated =await _employeeManager.UpdateAsync(employee);
                    if (isUpdated)
                    {
                        TempData["edited"]="Update Successfully";
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
            ViewBag.msg = msg;
            var employee1 = _mapper.Map<EmployeeEditVM>(employee);
            employee1.GenderList = new SelectList(await _genderManager.GetAllAsync(), "Id", "Name", employee.GenderId).ToList();
            employee1.ReligionList = new SelectList(await _religionManager.GetAllAsync(), "Id", "Name", employee.ReligionId).ToList();
            employee1.NationalityList = new SelectList(await _nationalityManager.GetAllAsync(), "Id", "Name", employee.NationalityId).ToList();
            employee1.EmpTypeList = new SelectList(await _empTypeManager.GetAllAsync(), "Id", "Name", employee.EmpTypeId).ToList();
            employee1.DesignationList = new SelectList(await _designationManager.GetAllAsync(), "Id", "DesignationName", employee.DesignationId).ToList();
            employee1.DivisionList = new SelectList(await _divisionManager.GetAllAsync(), "Id", "Name").ToList();
            employee1.BloodGroupList = new SelectList(await _bloodGroupManager.GetAllAsync(), "Id", "Name", employee.BloodGroupId).ToList();

            ViewData["PermanentDistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", employee.PermanentDistrictId);
            ViewData["PermanentUpazilaId"] = new SelectList(await _upazilaManager.GetAllAsync(), "Id", "Name", employee.PermanentUpazilaId);
            ViewData["PresentDistrictId"] = new SelectList(await _districtManager.GetAllAsync(), "Id", "Name", employee.PresentDistrictId);
            ViewData["PresentUpazilaId"] = new SelectList(await _upazilaManager.GetAllAsync(), "Id", "Name", employee.PresentUpazilaId);

            return View(employee1);
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
            bool isDeleted = await _employeeManager.RemoveAsync(emp);
            if (isDeleted)
            {
                TempData["deleted"] = "Deleted successfully";
            }
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
