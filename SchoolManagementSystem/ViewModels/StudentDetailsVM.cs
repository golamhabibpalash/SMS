using Models;
using SchoolManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.ViewModels
{
    public class StudentDetailsVM
    {
        public Student Student { get; set; }
        public List<StudentPayment> StudentPayments { get; set; }
    }
}