using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.Results
{
    public class RptExamResultVM
    {
        public string ExamName { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public List<AcademicResult> Results { get; set; }
    }
}
