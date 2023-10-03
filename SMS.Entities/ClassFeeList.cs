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

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;

        [Display (Name ="Class Name"), Required]
        public int AcademicClassId { get; set; }

        [Display(Name = "Academic Session"), Required]
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

        public StudentFeeHead StudentFeeHead { get; set; }
        public AcademicClass AcademicClass { get; set; }
    }
}
