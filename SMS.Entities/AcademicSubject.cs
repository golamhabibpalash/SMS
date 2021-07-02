using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicSubject
    {
        public int Id { get; set; }
        
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Subject Type")]
        public int AcademicSubjectTypeId { get; set; }

        [Display(Name ="Subject Code")]
        public int? SubjectCode { get; set; }

        [Display(Name ="Subject For")]
        public char SubjectFor { get; set; }

        [Display(Name = "Total Marks")]
        public double TotalMarks { get; set; }

        [Display(Name = "Is Active")]
        public bool Status { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited at")]
        public DateTime EditedAt { get; set; }

        public AcademicSubjectType AcademicSubjectType { get; set; }
    }
}
