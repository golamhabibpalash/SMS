using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMS.DB;
using System;
using Repositories;
using SMS.DAL.Contracts;
using SMS.BLL.Contracts;
using SMS.BLL.Managers;
using SMS.DAL.Repositories;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using SMS.Entities;

namespace SchoolManagementSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 5;
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddControllers().AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );

            //services.AddDatabaseDeveloperPageExceptionFilter();
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Accounts/AccessDenied");
                options.Cookie.Name = "Cookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = new PathString("/Accounts/Login");
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            

            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            services.AddRazorPages();

            services.AddSession(options =>
            {
                //options.Cookie.Name = ".AdventureWorks.Session";
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeManager, EmployeeManager>();

            services.AddScoped<IAcademicClassRepository, AcademicClassRepository>();
            services.AddScoped<IAcademicClassManager, AcademicClassManager>();

            services.AddScoped<IAcademicSessionRepository, AcademicSessionRepository>();
            services.AddScoped<IAcademicSessionManager, AcademicSessionManager>();

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentManager, StudentManager>();

            services.AddScoped<IInstituteRepository, InstituteRepository>();
            services.AddScoped<IInstituteManager, InstituteManager>();

            services.AddScoped<IAcademicSectionRepositoy, AcademicSectionRepository>();
            services.AddScoped<IAcademicSectionManager, AcademicSectionManager>();
            
            services.AddScoped<IAcademicSubjectRepository, AcademicSubjectRepository>();
            services.AddScoped<IAcademicSubjectManager, AcademicSubjectManager>();
            
            services.AddScoped<IAcademicSubjectTypeRepository, AcademicSubjectTypeRepository>();
            services.AddScoped<IAcademicSubjectTypeManager, AcademicSubjectTypeManager>();
            
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IDesignationManager, DesignationManager>();
            
            services.AddScoped<IDesignationTypeRepository, DesignationTypeRepository>();
            services.AddScoped<IDesignationTypeManager, DesignationTypeManager>();
            
            services.AddScoped<IEmpTypeRepository, EmpTypeRepository>();
            services.AddScoped<IEmpTypeManager, EmpTypeManager>();
            
            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<IGenderManager, GenderManager>();
            
            services.AddScoped<INationalityRepository, NationalityRepository>();
            services.AddScoped<INationalityManager, NationalityManager>();
            
            services.AddScoped<IStudentPaymentRepository, StudentPaymentRepository>();
            services.AddScoped<IStudentPaymentManager, StudentPaymentManager>();
            
            services.AddScoped<IStudentPaymentDetailsRepository, StudentPaymentDetailsRepository>();
            services.AddScoped<IStudentPaymentDetailsManager, StudentPaymentDetailsManager>();
            
            services.AddScoped<IStudentFeeHeadRepository, StudentFeeHeadRepository>();
            services.AddScoped<IStudentFeeHeadManager, StudentFeeHeadManager>();
            
            services.AddScoped<IReligionRepository, ReligionRepository>();
            services.AddScoped<IReligionManager, ReligionManager>();
            
            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped<IDivisionManager, DivisionManager>();
            
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IDistrictManager, DistrictManager>();
            
            services.AddScoped<IUpazilaRepository, UpazilaRepository>();
            services.AddScoped<IUpazilaManager, UpazilaManager>();
            
            services.AddScoped<IBloodGroupRepository, BloodGroupRepository>();
            services.AddScoped<IBloodGroupManager, BloodGroupManager>();
            
            services.AddScoped<IClassFeeListRepository, ClassFeeListRepository>();
            services.AddScoped<IClassFeeListManager, ClassFeeListManager>();

            services.AddScoped<IPhoneSMSRepository, PhoneSMSRepository>();
            services.AddScoped<IPhoneSMSManager, PhoneSMSManager>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IAttendanceManager, AttendanceManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseAuthentication();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Accounts}/{action=Login}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
}
