using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMS_App.Utilities.MACIPServices;
using SMS_App.ViewModels.Students;
using SMS.BLL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SMS_App.Controllers;
[Authorize(Roles = "SuperAdmin, Admin, Teacher")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStudentManager _studentManager;
    private readonly IEmployeeManager _employeeManager;
    private readonly IAcademicClassManager _academicClassManager;
    private readonly IAcademicSectionManager _academicSectionManager;
    private readonly IDesignationManager _designationManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IInstituteManager _instituteManager;
    private readonly IAttendanceManager _attendanceManager;
    private readonly IAttendanceMachineManager _attendanceMachineManager;
    private readonly IStudentPaymentManager _studentPaymentManager;
    private readonly ILogManager _logManager;
    private readonly IAcademicSessionManager _academicSessionManager;

    public HomeController(ILogger<HomeController> logger, IStudentManager studentManager, IEmployeeManager employeeManager, UserManager<ApplicationUser> userManager, IInstituteManager instituteManager, IAcademicClassManager academicClassManager, IDesignationManager designationManager, IAttendanceManager attendanceManager, IAttendanceMachineManager attendanceMachineManager, IStudentPaymentManager studentPaymentManager, ILogManager logManager, IAcademicSectionManager academicSectionManager, IAcademicSessionManager academicSessionManager)
    {
        _logger = logger;
        _studentManager = studentManager;
        _employeeManager = employeeManager;
        _userManager = userManager;
        _instituteManager = instituteManager;
        _academicClassManager = academicClassManager;
        _designationManager = designationManager;
        _attendanceManager = attendanceManager;
        _attendanceMachineManager = attendanceMachineManager;
        _studentPaymentManager = studentPaymentManager;
        _logManager = logManager;
        _academicSectionManager = academicSectionManager;
        _academicSessionManager = academicSessionManager;
    }

    public async Task<IActionResult> Index()
    {
        HttpContext.Session.SetString("macAddress", MACService.GetMAC());
        var user = await _userManager.GetUserAsync(User);
        HttpContext.Session.SetString("UserId", user.Id);

        Institute institute = await _instituteManager.GetFirstOrDefaultAsync();
        ViewBag.InstituteLogo = institute.Logo;
        ViewBag.InstituteName = institute.Name;

        DashboardIndexVM dashboard = new DashboardIndexVM();

        var currentSession = await _academicSessionManager.GetCurrentAcademicSessionAsync();
        dashboard.CurrentSession = currentSession;
        dashboard.CurrentSessionName = currentSession?.Name;

        var allStudents = await _studentManager.GetAllAsync();
        var sessionStudents = allStudents.Where(s => s.AcademicSessionId == currentSession?.Id && s.Status).ToList();

        var allEmployees = await _employeeManager.GetAllAsync();
        var activeEmployees = allEmployees.Where(e => e.Status).ToList();

        var allClasses = await _academicClassManager.GetAllAsync();
        var activeClasses = allClasses.Where(c => c.Status).ToList();

        var allSections = await _academicSectionManager.GetAllAsync();
        var activeSections = allSections.Where(s => s.Status).ToList();

        dashboard.TotalStudents = sessionStudents.Count;
        dashboard.TotalEmployees = activeEmployees.Count;
        dashboard.TotalClasses = activeClasses.Count;
        dashboard.TotalSections = activeSections.Count;

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var todayAbsentStudents = await _attendanceMachineManager.GetTodaysAbsentStudentAsync(today);
        var todayAbsentEmployees = await _attendanceMachineManager.GetTodaysAbsentEmployeeAsync(today);

        dashboard.TodayAbsentStudents = todayAbsentStudents?.Count ?? 0;
        dashboard.TodayPresentStudents = dashboard.TotalStudents - dashboard.TodayAbsentStudents;
        dashboard.TodayAbsentEmployees = todayAbsentEmployees?.Count ?? 0;
        dashboard.TodayPresentEmployees = dashboard.TotalEmployees - dashboard.TodayAbsentEmployees;

        dashboard.TodayAbsentStudentList = todayAbsentStudents?.Take(10).ToList() ?? new List<Student>();

        var todayCollections = await _studentPaymentManager.GetPaymentSummeryByDate(today);
        dashboard.TodayCollection = (decimal)(todayCollections?.Sum(c => c.Payments) ?? 0);
        dashboard.TodayCollections = todayCollections?.ToList() ?? new List<StudentPaymentSummeryVM>();

        var monthYear = DateTime.Today.ToString("yyyy-MM");
        var monthlyCollections = await _studentPaymentManager.GetPaymentSummeryByMonthYear(monthYear);
        dashboard.MonthlyCollection = (decimal)(monthlyCollections?.Sum(c => c.Payments) ?? 0);

        dashboard.ClassWiseStudentCounts = sessionStudents
            .GroupBy(s => s.AcademicClass?.Name ?? "N/A")
            .Select(g => new ClassWiseStudentCount { ClassName = g.Key, StudentCount = g.Count() })
            .OrderBy(c => c.ClassName)
            .ToList();

        dashboard.ClassWiseCollections = todayCollections
            .Select(c => new ClassWiseCollection { ClassName = c.AcademicClassName, Amount = (decimal)c.Payments })
            .OrderByDescending(c => c.Amount)
            .ToList();

        dashboard.RecentStudents = sessionStudents
            .OrderByDescending(s => s.CreatedAt)
            .Take(5)
            .ToList();

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

