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
        [Required]
        public string ExamTypeName { get; set; }
    }
}
