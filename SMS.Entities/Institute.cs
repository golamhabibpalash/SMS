using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Institute :CommonProps
    {
        public string Name { get; set; }
        public string EIIN { get; set; }
        public string Slogan { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string FavIcon { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }

        [Display(Name ="School Opening Time")]
        public string StartingTime { get; set; }

        [Display(Name = "School Closing Time")]
        public string ClosingTime { get; set; }

        [Display(Name = "Late Start Time(minutes)")]
        public int LateStartAfter { get; set; } = 30;
    }
}
