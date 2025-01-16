using SMS.App.ViewModels.PaymentVM;
using SMS.Entities;
using System.Collections.Generic;

namespace SMS.App.ViewModels
{
    public class StudentPaymentVM
    {
        public int Id { get; set; }
        public StudentPayment StudentPayment { get; set; }
        public List<StudentPayment> StudentCurrentPayments { get; set; } = new List<StudentPayment>(); // Session wise Current Payments
        public List<StudentPayment> StudentPreviousPayments { get; set; } = new List<StudentPayment>(); // Session wise Previous Payments
        public List<ClassFeeList> ClassFeeLists { get; set; }
        public int ClassFeeHeadId { get; set; }
        public int StudentId { get; set; }
        public bool IsSMSSend { get; set; }
        public List<PaymentItemVM> PaymentItemVMs { get; set; } = new List<PaymentItemVM>();
        public AcademicSession CurrentAcademicSession { get; set; }

        public SMS.App.ViewModels.PaymentVM.PaymentVM PaymentVM { get; set; } = new PaymentVM.PaymentVM();
    }
}
