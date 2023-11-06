using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ProjectModule:CommonProps
    {
        [Display(Name ="Module Name")]
        public string ModuleName { get; set; }
        public List<ProjectSubModule> SubModuleList { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
    }
}
