using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Report.ReportClass.Attendance
{
    public class MonthlyAttendance
    {
        public string Month { get; set; }
        public List<StudentAttendance> StudentAttendance { get; set; }
        public int MonthDayCount { get; set; }
    }
}
