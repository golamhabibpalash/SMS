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
    }
}
