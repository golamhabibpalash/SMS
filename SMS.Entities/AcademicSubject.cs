using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicSubject : CommonProps
    {
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Subject Type")]
        public int AcademicSubjectTypeId { get; set; }

        [Display(Name ="Subject Code")]
        public int? SubjectCode { get; set; }

        [Display(Name ="Subject For")]
        public char SubjectFor { get; set; } //High School, College, Primary School

        [Display(Name = "Total Marks")]
        public double TotalMarks { get; set; }

        [Display(Name = "Is Active")]
        public bool Status { get; set; }

        public AcademicSubjectType AcademicSubjectType { get; set; }
    }
}
