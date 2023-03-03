using System.ComponentModel.DataAnnotations;

namespace SMS.App.ViewModels.ReportVM
{
    public class rpt_Student_VM
    {
        [Display(Name="Academic Session")]
        public int AcademicSessionId { get; set; }

        [Display(Name = "Academic Class")] 
        public int AcademicClassId { get; set; }

        [Display(Name = "Academic Section")] 
        public int AcademicSectionId { get; set; }
    }
}
