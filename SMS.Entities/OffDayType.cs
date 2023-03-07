using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class OffDayType : CommonProps
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        [Display(Name ="Day Off Type")]
        public string OffDayTypeName { get; set; }
        public string Remarks { get; set; }
    }
}
