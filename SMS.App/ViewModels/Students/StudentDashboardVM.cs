using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.Students
{
    public class StudentDashboardVM
    {
        public ICollection<Student> Students { get; set; }
    }
}
