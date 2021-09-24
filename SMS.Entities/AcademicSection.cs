using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicSection : CommonProps
    {
        public string Name { get; set; }
        public bool Status { get; set; }

        [Required, Display(Name = "Class")]
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }

        [Required, Display(Name = "Academic Session")]
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

    }
}
