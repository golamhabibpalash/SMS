using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicSubjectType : CommonProps
    {
        [Display(Name ="Subject Type")]
        public string SubjectTypeName { get; set; }
        public bool Status { get; set; }
    }
}
