using System;

namespace SMS.App.ViewModels.AttendanceVM
{
    public class AttendanceMachineIndexVM
    {
        public int Id { get; set; }
        public string CardNo { get; set; }
        public DateTime PunchDateTime { get; set; }
        public string Name { get; set; }
        public string UserType { get; set; }
        public string MachineNo { get; set; }
    }
}
