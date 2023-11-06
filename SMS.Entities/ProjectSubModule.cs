using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Entities
{
    public class ProjectSubModule:CommonProps
    {

        [Display(Name = "Sub Module Name")] 
        public string SubModuleName { get; set; }
        public int ProjectModuleId { get; set; }
        public ProjectModule ProjectModule { get; set; }
        public bool Status { get; set; }
        public List<ClaimStores> ClaimStoresList { get; set; }
        public string Remarks { get; set; }
    }
}