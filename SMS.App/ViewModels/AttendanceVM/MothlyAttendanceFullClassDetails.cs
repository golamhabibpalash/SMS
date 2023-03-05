using System.Collections.Generic;

namespace SMS.App.ViewModels.AttendanceVM
{
    public class MonthlyAttendanceFullClassDetails
    {
        public string StudentName { get; set; }
        public IDictionary<int,bool> isPresents { get; set; }
    }
}
