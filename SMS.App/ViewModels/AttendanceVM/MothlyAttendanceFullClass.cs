using System.Collections.Generic;

namespace SMS.App.ViewModels.AttendanceVM
{
    public class MonthlyAttendanceFullClass
    {
        public string MonthName { get; set; }
        public string ClassName { get; set; }
        public List<MonthlyAttendanceFullClassDetails> MothlyAttendanceFullClassDetailses { get; set; }
    }
}
