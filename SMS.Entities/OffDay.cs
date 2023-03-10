using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class OffDay:CommonProps
    {
        [Display(Name = "Holiday Name")]
        public string OffDayName { get; set; }
        [Display(Name ="Start Date"), Required]
        public DateTime OffDayStartingDate { get; set; }
        [Display(Name ="End Date"),Required]
        public DateTime OffDayEndDate { get; set; }
        [Display(Name ="Days")]
        public int TotalDays { get; set; }
        public string Description { get; set; }
        [Display(Name = "Type of Holiday"), Required]
        public int OffDayTypeId { get; set; }
        public OffDayType OffDayType { get; set; }
    }
}
