using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentPayment
    {
        public int Id { get; set; }

        [Display(Name = "Student Name")]
        public int StudentId { get; set; }

        [Display(Name = "Receipt No"), Required]
        public string ReceiptNo { get; set; }

        [Display(Name = "Paid Date"), Required]
        [DataType(DataType.Date)]
        public DateTime PaidDate { get; set; }

        [Display(Name ="Waiver")]
        public double WaiverAmount { get; set; }

        [Display(Name ="Waiver For")]
        public string WaiverFor { get; set; }

        public string Attachment { get; set; }

        [Display(Name = "Total Paid")]
        public double TotalPayment { get; set; }
        public string Remarks { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

        public Student Student { get; set; }

        public List<StudentPaymentDetails> StudentPaymentDetails { get; set; }

    }
}
