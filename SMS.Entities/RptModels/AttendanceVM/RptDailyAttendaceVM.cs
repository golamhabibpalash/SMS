using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.AttendanceVM
{
    public class RptDailyAttendaceVM
    {
        public string ClassRoll { get; set; }
        public string StudentName { get; set; }
        public string AcademicClassName { get; set; }
        public string AcadmeicSectionName { get; set; }
        public string PhoneNo { get; set; }
        public string GuardianPhone { get; set; }
        public string Attendance { get; set; }
        public string AcademicClassId { get; set; }
        public string AcademicSectionId { get; set; }
    }
}
