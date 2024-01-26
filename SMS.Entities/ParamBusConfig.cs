namespace SMS.Entities
{
    public class ParamBusConfig:CommonProps
    {
        public int ParamSL { get; set; }
        public string ConfigName { get; set; }
        public string ParamValue { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}

//1 attendance Checkin Time Start
//2 attendance Checkin Time Stop
//3 attendance Checkout Time Start
//4 attendance Checkout TIme Stop
//5 Break Time Start
//6 Break Time Stop
//7 Attendance Summery SMS Time
//8 Absent Student Notification Time
//9 Daily Collection Summery SMS Notification Time
//10 Phone Numbers to send sms for daily attendance summary
//11 Emails to send email for daily attendance Summary
//12 Phone Numbers to send sms for daily Collection Summary
//13 Emails to send email for daily Collection Summary
//14 Phone Numbers to send sms for daily expense summary
//15 Emails to send email for daily expense summary
