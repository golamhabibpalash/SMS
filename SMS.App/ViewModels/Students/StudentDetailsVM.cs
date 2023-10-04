using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.Students
{
    public class StudentDetailsVM
    {
        public Student Student { get; set; }
        public IReadOnlyCollection<StudentPayment> StudentPayments { get; set; }
        public double TotalDue { get; set; }
        public double CurrentDue { get; set; }
        public List<StudentPaymentScheduleVM> StudentPaymentSchedules { get; set; } = new List<StudentPaymentScheduleVM>();
        public List<StudentPaymentSchedulePaidVM> StudentPaymentSchedulePaidVMs { get; set; } = new List<StudentPaymentSchedulePaidVM>();
        public List<AttendanceIndivisualVM> AttendanceDetails { get; set; }
    }

    public class AttendanceIndivisualVM
    {
        public string MonthName { get; set; }
        public int AttendanceCount { get; set; }
        public int TotalDays { get; set; }
        public int PresentPercentage { get; set; }
    }
}