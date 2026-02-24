using System.Collections.Generic;

namespace SMS.Entities.AdditionalModels.StudentVM;

public class StudentProfileVM
{
    public Student Student { get; set; }
    public ProfileAttendance Attendances { get; set; } = new ProfileAttendance();
    public ProfileResult Results { get; set; } = new ProfileResult();
    public ProfilePayment Payments { get; set; } = new ProfilePayment();
    public ProfileDocument Documents { get; set; } = new ProfileDocument();
}

public class ProfileAttendance
{
    public AcademicSession CurrentSession { get; set; }
    public double OverallAttendance { get; set; } = 0.0;
    public int DaysPresent { get; set; } = 0;
    public int DaysAbsent { get; set; } = 0;
    public int LateArrivals { get; set; } = 0;
    public List<MonthlyAttendance> MonthlyAttendances { get; set; } = new List<MonthlyAttendance>();
}
public class ProfileResult { }
public class ProfilePayment { }
public class ProfileDocument { }

public class MonthlyAttendance
{
    public string MonthName { get; set; }
    public int TotalDays { get; set; }
    public int DaysPresent { get; set; }
    public int DaysAbsent { get; set; }
    public int LateArrivals { get; set; }
    public int Percentage { get; set; }
    public string Status { get; set; }
    public string StatusColor { get; set; }
}

