using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using SMS.App.Utilities.EmailServices;
using static System.Net.Mime.MediaTypeNames;
using SMS.BLL.Contracts;

namespace SMS.App.Utilities.Automation
{
    public class AttendanceNotification : BackgroundService
    {
        private readonly ILogger<AttendanceNotification> _logger;
        private readonly IStudentManager _studentManager;
        public AttendanceNotification(ILogger<AttendanceNotification> logger, IStudentManager studentManager)
        {
            _logger = logger;
            _studentManager = studentManager;
        }
        DateTime StartingMailTime = new DateTime(0001, 01, 01, 08, 00, 00);
        DateTime StartingMailTime2 = new DateTime(0001, 01, 01, 09, 00, 00);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.ToString("hh:mm")== StartingMailTime.ToString("hh:mm"))
                {
                    var students = await _studentManager.GetAllAsync();
                    _logger.LogInformation("Welcome to Automation. Total Students");
                    //EmailService.SendEmail("golamhabibpalash@gmail.com", "Test Mail 1", "Welcome to Automation. Total Students" + students.Count);
                }
                await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);
            }
        }
    }
}
