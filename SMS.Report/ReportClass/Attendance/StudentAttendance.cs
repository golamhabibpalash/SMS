using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Report.ReportClass.Attendance
{
    public class StudentAttendance
    {
        public string Roll { get; set; }
        public string StudentName { get; set; }
        public List<Attendance> Attendances { get; set; }
        public int TotalAttendance { get; set; }
        public int AttendancePercentage { get; set; }
    }
}
