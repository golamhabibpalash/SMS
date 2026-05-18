using GHPEncryptDecript;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
using SMS_App.Utilities.ShortMessageService;
using SMS_App.Utilities.AutoMapperConfiguration;
using SMS_App.ViewModels.ModuleSubModuleVM;
using System;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// AES Key/IV
string aesKey = Environment.GetEnvironmentVariable("AES_KEY") ?? "1234567890123456";
string aesIv = Environment.GetEnvironmentVariable("AES_IV") ?? "1234567890123456";
byte[] key = Encoding.UTF8.GetBytes(aesKey);
byte[] iv = Encoding.UTF8.GetBytes(aesIv);

// Decrypt connection string
var connectionString =
    AesEncryptionHelper.Decrypt(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        key, iv
    );

// Hangfire configuration
bool hangfireEnabled = builder.Configuration.GetValue<bool>("Hangfire:IsEnabled");
string dashboardPath = builder.Configuration.GetValue<string>("Hangfire:DashboardPath") ?? "/hangfire";
int workerCount = builder.Configuration.GetValue<int>("Hangfire:WorkerCount");

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5);
        sqlOptions.CommandTimeout(180);
    });

    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// HTTP Client
builder.Services.AddHttpClient();

// Conditional Hangfire
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
        options.WorkerCount = workerCount;
    });
}

// Identity Configuration
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

// Fix Auto Logout Issue
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromHours(48);
});

// MVC + JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson(opt =>
        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Global Authorization
builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

// SESSION (Fixed)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);      // Session length
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = TimeSpan.FromHours(24);    // Important!
    options.Cookie.SameSite = SameSiteMode.Lax;        // Fix Chrome issues
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // safer for hosting
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;

    options.LoginPath = "/Accounts/Login";
    options.AccessDeniedPath = "/Accounts/AccessDenied";

    // Force cookie refresh
    options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;

    // Avoid chunked cookies if possible
    options.Cookie.Name = ".AspNetCore.Identity.Application";  
});




// Authorization Policies
builder.Services.AddAuthorization(o =>
{
    AuthorizationPolicies.ConfigureAuthorization(o);
});

// MVC / Views / Automapper
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
	builder.Services.AddAutoMapper(cfg => {
		cfg.AddProfile<AutoMapperProfile>();
	});
builder.Services.Addservices();

// SiteMap loader
builder.Services.AddSingleton<SiteMap>(provider =>
{
    var filePath = Path.Combine(builder.Environment.ContentRootPath, "siteMap.config");
    return SiteMapLoader.Load(filePath);
});

// FIX: Persist Data Protection Keys (required for shared hosting)
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "Keys")
    ))
    .SetApplicationName("SMS_App");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ORDER IS IMPORTANT
app.UseSession();      // Session BEFORE routing
app.UseRouting();

// Auth middlewares
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
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
    Console.WriteLine("Hangfire Disabled by configuration.");
}

// API controllers (attribute-routed)
app.MapControllers();

// AREA routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// default MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
