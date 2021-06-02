using Models;
using SchoolManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.ViewModels
{
    public class StudentyPaymentVM
    {
        public StudentPayment StudentPayment { get; set; }
        public List<StudentPayment> StudentPayments { get; set; }
        public List<ClassFeeList> ClassFeeLists { get; set; }
    }
}
