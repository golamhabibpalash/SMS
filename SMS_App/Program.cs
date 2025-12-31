using GHPEncryptDecript;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMS.DB;
using SMS.Entities;
using SMS_App.Configurations;
using SMS_App.Utilities.Automation.Hangfire;
using SMS_App.ViewModels.ModuleSubModuleVM;
using System;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// AES Key/IV
byte[] key = Encoding.UTF8.GetBytes("1234567890123456");
byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

// decrypt connection string
var connectionString =
    AesEncryptionHelper.Decrypt(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        key, iv
    );

// ⭐ read hangfire config without model class
bool hangfireEnabled = builder.Configuration.GetValue<bool>("Hangfire:IsEnabled");
string dashboardPath = builder.Configuration.GetValue<string>("Hangfire:DashboardPath") ?? "/hangfire";
int workerCount = builder.Configuration.GetValue<int>("Hangfire:WorkerCount");

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5);
        sqlOptions.CommandTimeout(180);
    });

    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddHttpClient();

// ⭐ conditional Hangfire registration
if (hangfireEnabled)
{
    builder.Services.AddHangfire(config =>
    {
        config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UseSqlServerStorage(connectionString,
                new SqlServerStorageOptions
                {
                    SchemaName = "hangfire",
                    QueuePollInterval = TimeSpan.FromSeconds(10)
                }
             );
    });

    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = workerCount; // background workers count
    });
}

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// MVC + JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson(opt =>
        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
});

builder.Services.ConfigureApplicationCookie(option =>
{
    option.Cookie.HttpOnly = true;
    option.ExpireTimeSpan = TimeSpan.FromHours(24);
    option.SlidingExpiration = true;

    //Paths
    option.LoginPath = "/Accounts/Login";
    option.AccessDeniedPath = "/Accounts/AccessDenied";

    //Security
    option.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    #if DEBUG
        option.Cookie.SecurePolicy = CookieSecurePolicy.None;
    #else
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    #endif
});

builder.Services.AddAuthorization(o =>
{
    AuthorizationPolicies.ConfigureAuthorization(o);
});

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Addservices();

builder.Services.AddSingleton<SiteMap>(provider =>
{
    var filePath = Path.Combine(builder.Environment.ContentRootPath, "siteMap.config");
    return SiteMapLoader.Load(filePath);
});

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

// conditionally enable dashboard
if (hangfireEnabled)
{
    var dashboardOptions = new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    };

    app.UseHangfireDashboard(dashboardPath, dashboardOptions);
}
else
{
    Console.WriteLine(" Hangfire Disabled by configuration.");
}

// AREA routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// default MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
