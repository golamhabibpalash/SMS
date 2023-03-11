﻿using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.App.ViewModels.ReportVM
{
    public class Rpt_Student_VM
    {
        public string ReportType { get; set; }
        [Display(Name="Academic Session")]
        public int AcademicSessionId { get; set; }

        [Display(Name = "Academic Class"), Required] 
        public int AcademicClassId { get; set; }

        [Display(Name = "Academic Section")] 
        public int AcademicSectionId { get; set; }

        public List<SelectListItem> AcademicSessionList { get; set; }
        public List<SelectListItem> AcademicClassList { get; set; }
        public List<SelectListItem> AcademicSectionList { get; set; }
    }
}
