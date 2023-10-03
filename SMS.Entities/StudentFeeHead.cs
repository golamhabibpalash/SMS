using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentFeeHead : CommonProps
    {
        public string Name { get; set; }
        public bool Repeatedly { get; set; }

        [Display(Name ="Yearly Frequency")]
        public int? YearlyFrequency { get; set; }
        [Display(Name ="Contra Fee Head")]
        public int? ContraFeeheadId { get; set; } = 0; //To create relation with other fee head
        [Display(Name = "Is Residential")]
        public bool IsResidential { get; set; } = false; //Is it use for Residential Student
    }
}
