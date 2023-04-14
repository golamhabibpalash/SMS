using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels
{
    public class rptStudentPaymentsVM
    {
        public string ReceiptNo { get; set; }
        public string PaidDate { get; set; }
        public string PaymentTypeName { get; set; }
        public double TotalPayment { get; set; }
        public string Remarks { get; set; }
    }
}
