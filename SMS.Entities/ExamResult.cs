using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ExamResult:CommonProps
    {
        public int AcademicExamId { get; set; }
        public AcademicExam AcademicExam { get; set; }
        public List<AcademicExamDetail> AcademicExamDetails { get; set; }
    }
}
