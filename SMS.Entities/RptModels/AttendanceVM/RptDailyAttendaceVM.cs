using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.AttendanceVM
{
    public class RptDailyAttendaceVM
    {
        public string CardNo { get; set; }
        public string Name { get; set; }
        public string Class_Designation { get; set; }
        public string Phone { get; set; }
        public string GuardianPhone { get; set; }
        public string PunchTime { get; set; }
    }
}
