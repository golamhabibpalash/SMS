using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ClaimStores:CommonProps
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public int SubModuleId { get; set; }
        public ProjectSubModule SubModule { get; set; }
    }
}
