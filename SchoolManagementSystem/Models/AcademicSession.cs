using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Models
{
    public class AcademicSession
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Session Name")]
        public string Name { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

        public List<ClassFeeList> StudentFeeLists { get; set; }
    }
}
