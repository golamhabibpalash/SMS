using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExamGroup : CommonProps
    {
        [Display(Name ="Exam Group Name")]
        public string ExamGroupName { get; set; }
        [Display(Name = "Academic Session")]
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }
        [Display(Name = "Exam Type")]
        public int academicExamTypeId { get; set; }
        public AcademicExamType academicExamType { get; set; }
        [Display(Name = "Exam Month")]
        public int ExamMonthId { get; set; }
        public bool Status { get; set; }
    }
}
