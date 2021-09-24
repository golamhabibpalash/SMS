using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class EmpType : CommonProps
    {

        [Display(Name ="Employee Type"),Required,StringLength(30)]
        public string Name { get; set; }


        public ICollection<Designation> Designations { get; set; }
    }
}
