using System;

namespace SMS.App.ViewModels.Students
{
    public class StudentActivateHistModel
    {
        public int StudentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime ActionDateTime { get; set; }
    }
}
