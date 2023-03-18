using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExamType : CommonProps
    {
        [Required, Display(Name ="Exam Type Name")]
        public string ExamTypeName { get; set; }
        
        [Display(Name ="Times in a Year"),Range(1,12)]
        public int CountInAYear { get; set; }
        public string Remarks { get; set; }
    }
}
