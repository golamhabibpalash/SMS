using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class DesignationType : CommonProps
    {
        [Display(Name = "Designation Type Name")]
        public string DesignationTypeName { get; set; }

        public List<Designation> Designations { get; set; }
    }
}
