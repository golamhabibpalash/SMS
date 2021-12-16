﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Designation : CommonProps
    {
        [Display(Name = "Designation")]
        public string DesignationName { get; set; }

        [Display(Name ="Designation Category")]
        public int DesignationTypeId { get; set; }

        [Display(Name ="Employee Type")]
        public int? EmpTypeId { get; set; }

        public DesignationType DesignationType { get; set; }
        public EmpType EmpType { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
