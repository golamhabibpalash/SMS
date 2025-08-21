using GHPEncryptDecript;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SMS.App.Configurations;
using SMS.App.Utilities.Automation.Hangfire;
using SMS.DB;
using SMS.Entities;
using System;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// Generate secure key and IV
byte[] key = Encoding.UTF8.GetBytes("1234567890123456");
byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");
var connectionString = AesEncryptionHelper.Decrypt(builder.Configuration.GetConnectionString("DefaultConnection"), key, iv);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(180); // Set command timeout to 120 seconds
        });
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddHttpClient();
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 5;
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
    .AddSessionStateTempDataProvider();


builder.Services.AddSessionConfiguration();
ServiceExtensions.ConfigureApplicationCookie(builder.Services);
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.ConfigureAuthorization(options);
});


builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(typeof(Program));

//builder.Services.AddHostedService<ScopedBackgroundService>();

builder.Services.Addservices();

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
var options = new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
};

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire", options);

// Area routes (more specific)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();
app.Run();