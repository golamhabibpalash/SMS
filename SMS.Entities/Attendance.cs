using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{ 
    public class Attendance : CommonProps
    {
        [StringLength(10)]
        public string CardNo { get; set; }

        [Display(Name ="Punching Time")]
        public DateTime PunchDatetime { get; set; }

        [Display(Name = "School Opening Time")]
        public string StartingTime { get; set; }

        [Display(Name = "School Closing Time")]
        public string ClosingTime { get; set; }

        [Display(Name = "Late Start Time(minutes)")]
        public int LateStartAfter { get; set; } = 30;

        [StringLength(5)]
        public string MachineNo { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
