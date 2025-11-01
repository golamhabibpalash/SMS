using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_App.Utilities.MACIPServices;
using SMS_App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SMS_App.Controllers;

[Authorize(Roles = "SuperAdmin, Admin")]
public class StudentFeeAllocationsController : Controller
{
    #region Fields
    private readonly IStudentFeeAllocationManager _studentFeeAllocationManager;
    private readonly IStudentFeeHeadManager _studentFeeHeadManager;
    private readonly IAcademicClassManager _academicClassManager;
    private readonly IStudentManager _student;
    private readonly IClassFeeListManager _classFeeListManager;
    private readonly IAcademicSessionManager _academicSessionManager;

    #endregion Fields

    #region Constructor
    public StudentFeeAllocationsController(IStudentFeeAllocationManager studentFeeAllocationManager, IStudentFeeHeadManager studentFeeHeadManager, IAcademicClassManager academicClassManager, IStudentManager student, IClassFeeListManager classFeeListManager, IAcademicSessionManager academicSessionManager = null)
    {
        _studentFeeAllocationManager = studentFeeAllocationManager;
        _studentFeeHeadManager = studentFeeHeadManager;
        _academicClassManager = academicClassManager;
        _student = student;
        _classFeeListManager = classFeeListManager;
        _academicSessionManager = academicSessionManager;
    }
    #endregion Constructor

    #region Methods
    // GET: StudentFeeAllocationsController

    [Authorize(Policy = "IndexStudentFeeAllocationsPolicy")]
    public async Task<ActionResult> Index()
    {
        StudentFeeAllocationVM studentFeeAllocationVM = new StudentFeeAllocationVM();
        studentFeeAllocationVM.StudentFeeAllocations = (List<StudentFeeAllocation>)await _studentFeeAllocationManager.GetAllAsync();
        //studentFeeAllocationVM.FeeList = new SelectList(await _studentFeeHeadManager.GetAllAsync(), "Id", "Name");
        var allClasses = await _academicClassManager.GetAllAsync();
        studentFeeAllocationVM.AcademicClassList = new SelectList(allClasses.Where(s => s.Status == true), "Id", "Name");

        return View(studentFeeAllocationVM);
    }

    [Authorize(Policy = "GroupStudentFeeAllocationsPolicy")]
    public async Task<ActionResult> FeeAllocationByGroup()
    {
        StudentFeeAllocationGroupVM studentFeeAllocationGroupVM = new StudentFeeAllocationGroupVM();
        var allSessions = await _academicSessionManager.GetAllAsync();
        studentFeeAllocationGroupVM.AcademicSessionList = new SelectList(allSessions.Where(s => s.Status == true), "Id", "Name");

        var allClasses = await _academicClassManager.GetAllAsync();
        studentFeeAllocationGroupVM.AcademicClassList = new SelectList(allClasses.Where(s => s.Status == true), "Id", "Name");

        return View(studentFeeAllocationGroupVM);
    }

    [Authorize(Policy = "GroupStudentFeeAllocationsPolicy")]
    [HttpPost]
    public async Task<ActionResult> FeeAllocationByGroup(StudentFeeAllocationGroupVM studentFeeAllocationGroupVM)
    {
        var notifications = string.Empty;
        int updated = 0;
        int inserted = 0;
        int inactive = 0;
        var classFeeList = await _classFeeListManager.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(studentFeeAllocationGroupVM.AcademicClassId, studentFeeAllocationGroupVM.FeeHeadId, studentFeeAllocationGroupVM.AcademicSessionId);
        if (studentFeeAllocationGroupVM.Students.Count>0)
        {
            List<StudentFeeAllocation> studentFeeAllocations = new List<StudentFeeAllocation>();    
            foreach (var student in studentFeeAllocationGroupVM.Students)
            {
                var existingAllocation =await _studentFeeAllocationManager.GetStudentFeeAllocationByUniqueIdClassFeeId(student.Student.UniqueId, classFeeList.FirstOrDefault().Id);
                if (student.IsChecked)
                {
                    if (existingAllocation!=null)
                    {
                        existingAllocation.AllocatedAmount = studentFeeAllocationGroupVM.AllocationAmount;
                        existingAllocation.EditedAt = DateTime.Now;
                        existingAllocation.EditedBy = HttpContext.Session.GetString("UserId");
                        existingAllocation.IsActive = true;
                        existingAllocation.MACAddress = MACService.GetMAC();
                        await _studentFeeAllocationManager.UpdateAsync(existingAllocation);
                        updated++;
                    }
                    else
                    {
                        StudentFeeAllocation studentFeeAllocation = new StudentFeeAllocation
                        {
                            StudentId = student.Student.Id,
                            UniqueId = student.Student.UniqueId,
                            StudentFeeHeadId = studentFeeAllocationGroupVM.FeeHeadId,
                            AllocatedAmount = studentFeeAllocationGroupVM.AllocationAmount,
                            ClassFeeListId = classFeeList.FirstOrDefault().Id,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            CreatedBy = HttpContext.Session.GetString("UserId"),
                            MACAddress = MACService.GetMAC()
                        };
                        studentFeeAllocations.Add(studentFeeAllocation);
                        inserted++;
                    }
                }
                else
                {
                    if (existingAllocation!=null)
                    {
                        existingAllocation.AllocatedAmount = studentFeeAllocationGroupVM.AllocationAmount;
                        existingAllocation.IsActive = false;
                        await _studentFeeAllocationManager.UpdateAsync(existingAllocation);
                        inactive++;
                    }
                }
            }
            await _studentFeeAllocationManager.AddRangeAsync(studentFeeAllocations);
        }

        var students = await _academicSessionManager.GetAllAsync();
        notifications = $"Total Added {inserted}; update {updated} and disabled {inactive}";
        TempData["notifications"] = notifications;
        return RedirectToAction("FeeAllocationByGroup");
    }

    // GET: StudentFeeAllocationsController/Details/5
    [Authorize(Policy = "DetailsStudentFeeAllocationsPolicy")]
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: StudentFeeAllocationsController/Create
    [Authorize(Policy = "CreateStudentFeeAllocationsPolicy")]
    public ActionResult Create()
    {
        return View();
    }

    // POST: StudentFeeAllocationsController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "CreateStudentFeeAllocationsPolicy")]
    public async Task<ActionResult> Create(StudentFeeAllocationVM studentFeeAllocationVM)
    {
        StudentFeeAllocation studentFeeAllocation = new StudentFeeAllocation();
        studentFeeAllocation = studentFeeAllocationVM.SFAllocation;
        if (ModelState.IsValid)
        {
            //check condition is duplicate or not?
            var existingFeeAllocation = await _studentFeeAllocationManager.GetStudentFeeAllocationByUniqueIdFeeHeadId(studentFeeAllocation.UniqueId, studentFeeAllocation.StudentFeeHeadId);
            if (existingFeeAllocation != null)
            {
                var exStudent = await _student.GetStudentByUniqueIdAsync(existingFeeAllocation.UniqueId);

                var feeHead = await _studentFeeHeadManager.GetByIdAsync(existingFeeAllocation.StudentFeeHeadId);
                TempData["failed"] = exStudent.Name + " is already allocated for " + feeHead.Name;
                return RedirectToAction("Index");
            }

            var student = await _student.GetStudentByUniqueIdAsync(studentFeeAllocation.UniqueId);
            var classFeeList = await _classFeeListManager.GetByClassIdAndFeeHeadIdAsync(student.AcademicClassId, studentFeeAllocation.StudentFeeHeadId, student.AcademicSessionId);
            if (classFeeList != null)
            {
                studentFeeAllocation.ClassFeeListId = classFeeList.Id;
            }
            var appUer = HttpContext.Session.GetString("UserId");
            if (appUer == null)
            {
                TempData["deleted"] = "User Not found! please login again";
                return RedirectToAction("Index");
            }
            studentFeeAllocation.CreatedAt = DateTime.Now;
            studentFeeAllocation.CreatedBy = HttpContext.Session.GetString("UserId");
            studentFeeAllocation.EditedAt = DateTime.Now;
            studentFeeAllocation.EditedBy = HttpContext.Session.GetString("UserId");
            studentFeeAllocation.MACAddress = MACService.GetMAC();
            try
            {
                bool isSaved = await _studentFeeAllocationManager.AddAsync(studentFeeAllocation);
                if (isSaved)
                {
                    TempData["created"] = "New Fee allocation added successfully";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        return RedirectToAction("Index");

    }

    // GET: StudentFeeAllocationsController/Edit/5
    [Authorize(Policy = "EditStudentFeeAllocationsPolicy")]
    public ActionResult Edit(int id)
    {
        return View();
    }

    // POST: StudentFeeAllocationsController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "EditStudentFeeAllocationsPolicy")]
    public async Task<ActionResult> Edit(int id, StudentFeeAllocationVM studentFeeAllocationVM)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingAllocation = await _studentFeeAllocationManager.GetByIdAsync(studentFeeAllocationVM.SFAllocation.Id);
                if (existingAllocation.Id != studentFeeAllocationVM.SFAllocation.Id)
                {
                    TempData["deleted"] = "Data is miss matched";
                    return RedirectToAction("Index");
                }
                if (existingAllocation.StudentId == studentFeeAllocationVM.SFAllocation.StudentId && existingAllocation.IsActive == studentFeeAllocationVM.SFAllocation.IsActive && existingAllocation.AllocatedAmount == studentFeeAllocationVM.SFAllocation.AllocatedAmount && existingAllocation.StudentFeeHeadId == studentFeeAllocationVM.SFAllocation.StudentFeeHeadId)
                {
                    TempData["error"] = "Nothing Change";
                }
                else
                {
                    existingAllocation.EditedAt = DateTime.Now;
                    existingAllocation.EditedBy = HttpContext.Session.GetString("UserId");
                    existingAllocation.MACAddress = MACService.GetMAC();
                    existingAllocation.StudentId = studentFeeAllocationVM.SFAllocation.StudentId;
                    existingAllocation.IsActive = studentFeeAllocationVM.SFAllocation.IsActive;
                    existingAllocation.AllocatedAmount = studentFeeAllocationVM.SFAllocation.AllocatedAmount;
                    existingAllocation.StudentFeeHeadId = studentFeeAllocationVM.SFAllocation.StudentFeeHeadId;
                    bool isUpdated = await _studentFeeAllocationManager.UpdateAsync(existingAllocation);
                    if (isUpdated)
                    {
                        TempData["updated"] = "Successfully updated";
                    }
                }
            }
            catch
            {
                return View();
            }
        }
        return RedirectToAction("index");
    }

    // GET: StudentFeeAllocationsController/Delete/5
    [Authorize(Policy = "DeleteStudentFeeAllocationsPolicy")]
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: StudentFeeAllocationsController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "DeleteStudentFeeAllocationsPolicy")]
    public async Task<ActionResult> Delete(int id, IFormCollection formCollection)
    {
        //StudentFeeAllocationId
        try
        {
            var existingAllocation = await _studentFeeAllocationManager.GetByIdAsync(Convert.ToInt32(formCollection["StudentFeeAllocationId"]));
            if (existingAllocation.Id != Convert.ToInt32(formCollection["StudentFeeAllocationId"]))
            {
                TempData["error"] = "Data is miss matched";
                return RedirectToAction("Index");
            }
            var isDeleted = await _studentFeeAllocationManager.RemoveAsync(existingAllocation);
            if (isDeleted)
            {
                TempData["deleted"] = "Data Deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            TempData["error"] = "Exception Occured";
            return RedirectToAction("Index");
        }
    }

    #endregion Methods
}

