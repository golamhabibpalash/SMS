using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class MobileSMSControl:CommonProps
    {
        public bool SMSService { get; set; }

        public bool AttendanceSMSService { get; set; }
        public bool GirlsStudentSMSServiceIn { get; set; }
        public bool GirlsStudentSMSServiceOut { get; set; }
        public bool BoysStudentSMSServiceIn { get; set; }
        public bool BoysStudentSMSServiceOut { get; set; }
        public bool EmployeeSMSServiceIn { get; set; }
        public bool EmployeeSMSServiceOut { get; set; }

        public bool PaymentSMSService { get; set; }
    }
}
