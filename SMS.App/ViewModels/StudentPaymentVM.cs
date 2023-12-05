using SMS.App.ViewModels.PaymentVM;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels
{
    public class StudentPaymentVM
    {
        public int Id { get; set; } 
        public StudentPayment StudentPayment { get; set; }
        public List<StudentPayment> StudentPayments { get; set; }
        public List<ClassFeeList> ClassFeeLists { get; set; }
        public int ClassFeeHeadId { get; set; }
        public int StudentId { get; set; }
        public bool IsSMSSend { get; set; }
        public List<PaymentItemVM>   PaymentItemVMs { get; set; } = new List<PaymentItemVM>();
    }
}
