using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Upazila : CommonProps
    {

        [Display(Name = "Upazila Name")]
        public string Name { get; set; }

        [Display(Name = "District")]
        public int DistrictId { get; set; }
        public District District { get; set; }

        public bool Status { get; set; }

    }
}
