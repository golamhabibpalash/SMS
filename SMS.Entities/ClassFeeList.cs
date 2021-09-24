using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ClassFeeList : CommonProps
    {
        [Display(Name ="Student Fee Head")]
        public int StudentFeeHeadId { get; set; }

        public double Amount { get; set; }

        [Display (Name ="Class Name"), Required]
        public int AcademicClassId { get; set; }

        public StudentFeeHead StudentFeeHead { get; set; }
        public AcademicClass AcademicClass { get; set; }
    }
}
