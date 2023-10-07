using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.AdditionalModels
{
    public class StudentPaymentSummerySMS_VM
    {
        public decimal ResidentialPayment { get; set; }
        public decimal NonResidentialPayment { get; set; }
    }
}
