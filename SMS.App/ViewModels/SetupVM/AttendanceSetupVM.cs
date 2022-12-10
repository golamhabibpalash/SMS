namespace SMS.App.ViewModels.SetupVM
{
    public class AttendanceSetupVM
    {
        public int Id { get; set; }
        public bool AttendanceSMSService { get; set; }
        public bool CheckInSMS { get; set; }
        public bool CheckOutSMS { get; set; }
        public bool CheckInSMSEmployees { get; set; }
        public bool CheckInSMSStudentBoys { get; set; }
        public bool CheckInSMSStudentGirls { get; set; }
        public bool CheckOutSMSEmployees { get; set; }
        public bool CheckOutSMSStudentBoys { get; set; }
        public bool CheckOutSMSStudentGirls { get; set; }
        public bool CheckInSMSSummary { get; set; }
        public bool AbsentNotification { get; set; }
    }
}
