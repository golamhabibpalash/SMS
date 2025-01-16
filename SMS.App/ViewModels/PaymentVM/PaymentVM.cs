using System.Collections.Generic;

namespace SMS.App.ViewModels.PaymentVM
{
    public class PaymentVM
    {
        public List<SinglePaymentVM> Payments { get; set; } = new List<SinglePaymentVM>();
    }
    public class SinglePaymentVM
    {
        public string PaymentsTitle { get; set; }
        public string AcademicSession { get; set; }
        public List<SessionWisePaymentVM> SessionWisePaymentVMs { get; set; } = new List<SessionWisePaymentVM>();
    }

    public class SessionWisePaymentVM
    {
        public string FeeHeadName { get; set; }
        public double Amount { get; set; }
        public double PaidAmount { get; set; }
        public double Balance { get; set; }
        public string Status { get; set; }
    }
}
