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
        try
        {
            HttpContext.Session.SetString("macAddress", MACService.GetMAC());
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            
            HttpContext.Session.SetString("UserId", user.Id);

            var institute = await _instituteManager.GetFirstOrDefaultAsync();
            ViewBag.InstituteLogo = institute?.Logo;
            ViewBag.InstituteName = institute?.Name;

            var dashboard = new DashboardIndexVM();

            var currentSession = await _academicSessionManager.GetCurrentAcademicSessionAsync();
            
            _logger.LogInformation("Current Session: {Session}", currentSession?.Name);
            
            if (currentSession == null)
            {
                dashboard.CurrentSessionName = "No Active Session";
                ViewBag.Warning = "No active academic session found. Please create one.";
                return View(dashboard);
            }
            
            dashboard.CurrentSession = currentSession;
            dashboard.CurrentSessionName = currentSession.Name;

            _logger.LogInformation("Fetching students for session: {SessionId}", currentSession.Id);
            
            var allStudents = await _studentManager.GetAllAsync();
            _logger.LogInformation("Total students in DB: {Count}", allStudents.Count);
            
            // Get students in current session - count ALL students in session (both active and inactive for dashboard)
            var sessionStudents = allStudents.Where(s => s.AcademicSessionId == currentSession.Id).ToList();
            _logger.LogInformation("Students in current session (all): {Count}", sessionStudents.Count);
            
            // Active students only
            var activeSessionStudents = sessionStudents.Where(s => s.Status).ToList();
            _logger.LogInformation("Active students in current session: {Count}", activeSessionStudents.Count);

            var allEmployees = await _employeeManager.GetAllAsync();
            var activeEmployees = allEmployees.Where(e => e.Status).ToList();

            var allClasses = await _academicClassManager.GetAllAsync();
            var activeClasses = allClasses.Where(c => c.Status).ToList();

            var allSections = await _academicSectionManager.GetAllAsync();
            var activeSections = allSections.Where(s => s.Status).ToList();

            dashboard.TotalStudents = activeSessionStudents.Count;
            dashboard.TotalEmployees = activeEmployees.Count;
            dashboard.TotalClasses = activeClasses.Count;
            dashboard.TotalSections = activeSections.Count;
        
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            _logger.LogInformation("Today's date: {Date}", today);
            
            var todayAbsentStudents = await _attendanceMachineManager.GetTodaysAbsentStudentAsync(today);
            var todayAbsentEmployees = await _attendanceMachineManager.GetTodaysAbsentEmployeeAsync(today);

            dashboard.TodayAbsentStudents = todayAbsentStudents?.Count ?? 0;
            dashboard.TodayPresentStudents = dashboard.TotalStudents - dashboard.TodayAbsentStudents;
            dashboard.TodayAbsentEmployees = todayAbsentEmployees?.Count ?? 0;
            dashboard.TodayPresentEmployees = dashboard.TotalEmployees - dashboard.TodayAbsentEmployees;

            dashboard.TodayAbsentStudentList = todayAbsentStudents?.Take(10).ToList() ?? new List<Student>();

            // Get Today's Collection
            var todayCollections = await _studentPaymentManager.GetPaymentSummeryByDate(today);
            decimal todayTotal = 0;
            if (todayCollections != null && todayCollections.Any())
            {
                foreach (var item in todayCollections)
                {
                    todayTotal += Convert.ToDecimal(item.Payments);
                }
            }
            dashboard.TodayCollection = todayTotal;
            dashboard.TodayCollections = todayCollections?.ToList() ?? new List<StudentPaymentSummeryVM>();

            // Get Monthly Collection
            var monthYear = DateTime.Today.ToString("yyyy-MM");
            var monthlyCollections = await _studentPaymentManager.GetPaymentSummeryByMonthYear(monthYear);
            decimal monthTotal = 0;
            if (monthlyCollections != null && monthlyCollections.Any())
            {
                foreach (var item in monthlyCollections)
                {
                    monthTotal += Convert.ToDecimal(item.Payments);
                }
            }
            dashboard.MonthlyCollection = monthTotal;

            dashboard.ClassWiseStudentCounts = activeSessionStudents
                .GroupBy(s => s.AcademicClass?.Name ?? "N/A")
                .Select(g => new ClassWiseStudentCount { ClassName = g.Key, StudentCount = g.Count() })
                .OrderBy(c => c.ClassName)
                .ToList();

            dashboard.ClassWiseCollections = todayCollections
                .Select(c => new ClassWiseCollection { ClassName = c.AcademicClassName, Amount = (decimal)c.Payments })
                .OrderByDescending(c => c.Amount)
                .ToList();

            dashboard.RecentStudents = activeSessionStudents
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .ToList();

            return View(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            var dashboard = new DashboardIndexVM();
            dashboard.CurrentSessionName = "Error";
            ViewBag.Error = "Error loading dashboard data: " + ex.Message;
            return View(dashboard);
        }
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