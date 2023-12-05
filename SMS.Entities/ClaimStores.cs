using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ClaimStores:CommonProps
    {
        [Display(Name="Claim Type")]
        public string ClaimType { get; set; }
        [Display(Name = "Claim Value")]
        public string ClaimValue { get; set; }
        [Display(Name = "Sub-Module")]
        public int SubModuleId { get; set; }
        public ProjectSubModule SubModule { get; set; }
    }
}
