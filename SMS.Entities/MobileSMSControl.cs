using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    internal class MobileSMSControl:CommonProps
    {
        public bool SMSService { get; set; }
        public bool EmployeeSMSService { get; set; }
        public bool BoysStudentSMSService { get; set; }
        public bool GirlsStudentSMSService { get; set; }
        public bool AttendanceSMSService { get; set; }
    }
}
