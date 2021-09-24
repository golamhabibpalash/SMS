using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class District : CommonProps
    {
        public string Name { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public bool Status { get; set; }
    }
}
