using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.Students
{
    public class DashboardIndexVM
    {
        public ICollection<Student> Students { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<AcademicClass> Classes { get; set; }
        public ICollection<Designation> Designations { get; set; }
    }
}
