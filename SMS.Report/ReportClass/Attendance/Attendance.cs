using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Report.ReportClass.Attendance
{
    public class Attendance
    {
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; } = false;
        public bool IsHoliday { get; set; } = false;
        public Holiday Holiday { get; set; } = new Holiday();
    }
}
