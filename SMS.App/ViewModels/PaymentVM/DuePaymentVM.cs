using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using System.Collections.Generic;

namespace SMS.App.ViewModels.PaymentVM
{
    public class DuePaymentVM
    {
        public AcademicClass AcademicClass { get; set; }
        public int AcademicClassId { get; set; }
        public int? AcademicSectionId { get; set; }
        public List<DuePaymentDetailsVM> DuePayments { get; set; }
        public List<SelectListItem> AcademicClassList { get; set; }
        public List<SelectListItem> AcademicSectionList { get; set; }
        public Institute Institute { get; set; }
        public double GrandTotal { get; set; }
    }
}
