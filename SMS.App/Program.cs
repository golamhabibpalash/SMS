using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.DB;
using SMS.Entities;
using System;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 5;
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );



builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Accounts/AccessDenied");
    options.Cookie.Name = "Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.LoginPath = new PathString("/Accounts/Login");
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();



// Add services to the container.
builder.Services.AddRazorPages();

//builder.Services.AddAutoMapper();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeManager, EmployeeManager>();

builder.Services.AddScoped<IAcademicClassRepository, AcademicClassRepository>();
builder.Services.AddScoped<IAcademicClassManager, AcademicClassManager>();

builder.Services.AddScoped<IAcademicSessionRepository, AcademicSessionRepository>();
builder.Services.AddScoped<IAcademicSessionManager, AcademicSessionManager>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentManager, StudentManager>();

builder.Services.AddScoped<IInstituteRepository, InstituteRepository>();
builder.Services.AddScoped<IInstituteManager, InstituteManager>();

builder.Services.AddScoped<IAcademicSectionRepositoy, AcademicSectionRepository>();
builder.Services.AddScoped<IAcademicSectionManager, AcademicSectionManager>();

builder.Services.AddScoped<IAcademicSubjectRepository, AcademicSubjectRepository>();
builder.Services.AddScoped<IAcademicSubjectManager, AcademicSubjectManager>();

builder.Services.AddScoped<IAcademicSubjectTypeRepository, AcademicSubjectTypeRepository>();
builder.Services.AddScoped<IAcademicSubjectTypeManager, AcademicSubjectTypeManager>();

builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IDesignationManager, DesignationManager>();

builder.Services.AddScoped<IDesignationTypeRepository, DesignationTypeRepository>();
builder.Services.AddScoped<IDesignationTypeManager, DesignationTypeManager>();

builder.Services.AddScoped<IEmpTypeRepository, EmpTypeRepository>();
builder.Services.AddScoped<IEmpTypeManager, EmpTypeManager>();

builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IGenderManager, GenderManager>();

builder.Services.AddScoped<INationalityRepository, NationalityRepository>();
builder.Services.AddScoped<INationalityManager, NationalityManager>();

builder.Services.AddScoped<IStudentPaymentRepository, StudentPaymentRepository>();
builder.Services.AddScoped<IStudentPaymentManager, StudentPaymentManager>();

builder.Services.AddScoped<IStudentPaymentDetailsRepository, StudentPaymentDetailsRepository>();
builder.Services.AddScoped<IStudentPaymentDetailsManager, StudentPaymentDetailsManager>();

builder.Services.AddScoped<IStudentFeeHeadRepository, StudentFeeHeadRepository>();
builder.Services.AddScoped<IStudentFeeHeadManager, StudentFeeHeadManager>();

builder.Services.AddScoped<IReligionRepository, ReligionRepository>();
builder.Services.AddScoped<IReligionManager, ReligionManager>();

builder.Services.AddScoped<IDivisionRepository, DivisionRepository>();
builder.Services.AddScoped<IDivisionManager, DivisionManager>();

builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IDistrictManager, DistrictManager>();

builder.Services.AddScoped<IUpazilaRepository, UpazilaRepository>();
builder.Services.AddScoped<IUpazilaManager, UpazilaManager>();

builder.Services.AddScoped<IBloodGroupRepository, BloodGroupRepository>();
builder.Services.AddScoped<IBloodGroupManager, BloodGroupManager>();

builder.Services.AddScoped<IClassFeeListRepository, ClassFeeListRepository>();
builder.Services.AddScoped<IClassFeeListManager, ClassFeeListManager>();

builder.Services.AddScoped<IPhoneSMSRepository, PhoneSMSRepository>();
builder.Services.AddScoped<IPhoneSMSManager, PhoneSMSManager>();

builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAttendanceManager, AttendanceManager>();

builder.Services.AddScoped<IAttendanceMachineRepository, AttendanceMachineRepository>();
builder.Services.AddScoped<IAttendanceMachineManager, AttendanceMachineManager>();

builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<IChapterManager, ChapterManager>();

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionManager, QuestionManager>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();