using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicClass : CommonProps
    {
        [Required, Display(Name ="Class Name")]
        public string Name { get; set; }

        //[Display(Name = "Academic Session"), Required]
        //public int AcademicSessionId { get; set; }

        [Display(Name ="Class Position"), Range(1,12),Required]
        public int ClassSerial { get; set; }

        //public AcademicSession AcademicSession { get; set; }

        public string Description { get; set; }
        public bool Status { get; set; }


        public List<Student> Students { get; set; }
        public List<AcademicSection> AcademicSections { get; set; }
        public List<ClassFeeList> StudentFeeLists { get; set; }
    }
}
