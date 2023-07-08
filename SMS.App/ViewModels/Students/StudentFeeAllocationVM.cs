using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using System.Collections.Generic;

namespace SMS.App.ViewModels.Students
{
    public class StudentFeeAllocationVM
    {
        public List<StudentFeeAllocation> StudentFeeAllocations { get; set; } = new List<StudentFeeAllocation>();
        public StudentFeeAllocation SFAllocation { get; set; } = new StudentFeeAllocation();
        public SelectList FeeList { get; set; }
        public SelectList AcademicClassList { get; set; }
    }
}
