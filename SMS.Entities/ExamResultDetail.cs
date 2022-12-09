using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ExamResultDetail : CommonProps
    {
        public int ExamResultId { get; set; }
        public ExamResult ExamResult { get; set; }
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public double Marks { get; set; }
    }
}
