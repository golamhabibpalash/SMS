using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentPaymentDetails
    {
        public int Id { get; set; }

        public int StudentPaymentId { get; set; }

        public int StudentFeeHeadId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

        public StudentFeeHead StudentFeeHead { get; set; }
        public StudentPayment StudentPayment { get; set; }

    }
}
