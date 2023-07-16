using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExam : CommonProps
    {
        [Display(Name ="Exam Group")]
        public int AcademicExamGroupId { get; set; }
        public AcademicExamGroup AcademicExamGroup { get; set; }
        [Display(Name = "Academic Class")] 
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }
        [Display(Name = "Academic Section")]
        public int? AcademicSectionId { get; set; }
        public AcademicSection AcademicSection { get; set; }
        [Display(Name = "Exam Teacher")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        [Display(Name = "Academic Subject")]
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; }
        public bool Status { get; set; }
        public List<AcademicExamDetail> AcademicExamDetails { get; set; }
    }
}
