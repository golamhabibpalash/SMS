using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace SMS.App.ViewModels.AttendanceVM
{
    public class DailyCheckoutReportVM
    {
        public string ReportFor { get; set; } //student or staff
        public DateTime ReportDate { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public List<SelectListItem> AcademicClassList { get; set; }
    }
}
