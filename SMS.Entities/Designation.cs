using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Designation
    {
        public int Id { get; set; }

        [Display(Name = "Designation")]
        public string DesignationName { get; set; }

        [Display(Name ="Designation Type")]
        public int DesignationTypeId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited At")]
        public DateTime EditedAt { get; set; }

        public DesignationType DesignationType { get; set; }
    }
}
