using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExam : CommonProps
    {
        [Display(Name ="Exam Name")]
        public string ExamName { get; set; }
        [Display(Name = "Exam Type")]
        public int AcademicExamTypeId { get; set; }

        public int? AcademicSectionId { get; set; }

        [Display(Name = "Academic Session")]
        public int AcademicSessionId { get; set; }
        [Display(Name = "Academic Subject")]
        public int AcademicSubjectId { get; set; }
        [Display(Name = "Teacher")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int MonthId { get; set; }
        public AcademicExamType AcademicExamType { get; set; }
        public AcademicSession AcademicSession { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public AcademicSection AcademicSection { get; set; }
        public List<AcademicExamDetail> AcademicExamDetails { get; set; }
        [Display(Name = "Total Marks")]
        public double TotalMarks { get; set; }

        public bool IsActive { get; set; }

    }
}
