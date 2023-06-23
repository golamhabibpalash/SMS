using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentPaymentDetails : CommonProps
    {
        public int StudentPaymentId { get; set; }
        public int StudentFeeHeadId { get; set; }
        public double PaidAmount { get; set; }
        public StudentFeeHead StudentFeeHead { get; set; }
        public StudentPayment StudentPayment { get; set; }

    }
}
