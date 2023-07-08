using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentFeeAllocation:CommonProps
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int StudentFeeHeadId { get; set; }
        public StudentFeeHead StudentFeeHead { get; set; }
        public double AllocatedAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public string FeeAllocationApplication { get; set; } =string.Empty;
    }
}
