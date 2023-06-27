using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.StudentPayment
{
    
    public class RptPaymentReceiptVM
    {
        public string ReceiptNo { get; set; }
        public string Student_Name { get; set; }
        public string Class_Name { get; set; }
        public DateTime PaidDate { get; set; }
        public int ClassRoll { get; set; }
        public string Section_Name { get; set; }
        public string Fee_Head { get; set; }
        public double PaidAmount { get; set; }
        public double TotalPayment { get; set; }
    }
}
