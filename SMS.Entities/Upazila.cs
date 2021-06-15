using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Upazila
    {
        public int Id { get; set; }

        [Display(Name = "Upazila Name")]
        public string Name { get; set; }

        [Display(Name = "District")]
        public int DistrictId { get; set; }
        public District District { get; set; }

        public bool Status { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Edited By")]
        public string EditedBy { get; set; }

        [Display(Name = "Edited at")]
        public DateTime EditedAt { get; set; }
    }
}
