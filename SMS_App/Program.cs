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

// COOKIE FIXED (Identity Cookie)
builder.Services.ConfigureApplicationCookie(option =>
{
    option.Cookie.HttpOnly = true;
    option.ExpireTimeSpan = TimeSpan.FromHours(24);     // Cookie length
    option.Cookie.MaxAge = TimeSpan.FromHours(24);      // Important!
    option.SlidingExpiration = true;
    option.Cookie.SameSite = SameSiteMode.Lax;
    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    option.LoginPath = "/Accounts/Login";
    option.AccessDeniedPath = "/Accounts/AccessDenied";

    // Force cookie refresh
    option.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
});

// Authorization Policies
builder.Services.AddAuthorization(o =>
{
    AuthorizationPolicies.ConfigureAuthorization(o);
});

// MVC / Views / Automapper
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Addservices();

// SiteMap loader
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
