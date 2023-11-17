using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Institute :CommonProps
    {
        [Display(Name ="Institute Name")]
        public string Name { get; set; }
        [Display(Name ="Short Name")]
        public string ShortName { get; set; }
        public string EIIN { get; set; }
        [Display(Name ="Branch Name")]
        public string BranchName { get; set; }
        [Phone,Display(Name ="Phone(1)")]
        public string Phone1 { get; set; }
        [Phone, Display(Name = "Phone(2)")]
        public string Phone2 { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Slogan { get; set; }
        public string Logo { get; set; }
        [Display(Name ="Title Icon")]
        public string FavIcon { get; set; }

        public string Address { get; set; }
        public string Banner { get; set; }

        [Display(Name ="Institute Opening Time")]
        public DateTime StartingTime { get; set; }

        [Display(Name = "Institute Closing Time")]
        public DateTime ClosingTime { get; set; }

        [Display(Name = "Late Time")]
        public DateTime LateTime { get; set; }
    }
}
