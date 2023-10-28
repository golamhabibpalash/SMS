using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class SetupMobileSMS:CommonProps
    {
        public bool SMSService { get; set; }

        #region Attendance SMS Service
        public bool AttendanceSMSService { get; set; }

        public bool CheckInSMSService { get; set; }
        public bool CheckInSMSServiceForMaleStudent { get; set; }
        public bool CheckInSMSServiceForGirlsStudent { get; set; }
        public bool CheckInSMSServiceForEmployees { get; set; }
        public bool CheckInSMSSummary { get; set; }
        public bool CheckOutSMSService { get; set; }
        public bool CheckOutSMSServiceForMaleStudent { get; set; }
        public bool CheckOutSMSServiceForGirlsStudent { get; set; }
        public bool CheckOutSMSServiceForEmployees { get; set; }
        #endregion

        #region Payment SMS Service
        public bool PaymentSMSService { get; set; }
        public bool DailyCollectionSMSService { get; set; }
        #endregion

        #region Administration SMS Service
        public bool AdministrativeSMSService { get; set; }
        #endregion

        #region Absent Notification SMS
        public bool AbsentNotification { get; set; }
        public bool AbsentNotificationStudent { get; set; }
        public bool AbsentNotificationEmployee { get; set; }
        #endregion
    }
}
