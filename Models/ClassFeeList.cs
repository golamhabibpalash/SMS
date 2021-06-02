using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ClassFeeList
    {
        public int Id { get; set; }

        [Display(Name ="Student Fee Head")]
        public int StudentFeeHeadId { get; set; }

        public double Amount { get; set; }

        [Display (Name ="Class Name"), Required]
        public int AcademicClassId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

        public StudentFeeHead StudentFeeHead { get; set; }
        public AcademicClass AcademicClass { get; set; }
    }
}
