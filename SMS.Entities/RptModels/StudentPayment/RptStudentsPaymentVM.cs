using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.StudentPayment
{
    public class RptStudentsPaymentVM
    {
        public string ClassRoll { get; set; }
        public string StudentName { get; set; }
        public string AcademicSection { get; set; } = string.Empty;
        public string PaymentType { get; set; }
        public string ReceiptNo { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string PaidDate { get; set; }
        public double TotalPayment { get; set; }
        public  string AcademicClassId  { get; set; } = string.Empty;
        public string AcademicSectionId { get; set; } = string.Empty;
        public string AcademicClassName { get; set; }
        public bool IsResidential { get; set; }

    }
}
