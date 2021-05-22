using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Models
{
    public class Institute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO { get; set; }
        public string EIIN { get; set; }
        public string Slogan { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

    }
}
