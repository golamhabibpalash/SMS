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

            //services.AddDatabaseDeveloperPageExceptionFilter();

            

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
}
