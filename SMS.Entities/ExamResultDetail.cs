using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        
        public double ObtainMark { get; set; }
        public double TotalMark { get; set; }
        public double GPA { get; set; }
        public string Grade { get; set; }
    }
}
