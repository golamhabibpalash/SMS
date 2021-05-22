using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Models
{
    public class AcademicSubject
    {
        public int Id { get; set; }
        
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Display(Name = "Subject Type")]
        public int AcademicSubjectTypeId { get; set; }

        public double TotalMarks { get; set; }

        public bool IsOptional { get; set; }

        [Display(Name = "Active/Inactive")]
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
