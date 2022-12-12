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
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }
        
        public int StudentId { get; set; }
        public Student Student { get; set; }
       
        public int ExamId { get; set; }
        public AcademicExam Exam { get; set; }

        public double TotalMarks { get; set; }

        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public List<ExamResultDetail> ExamResultDetails { get; set; }
    }
}
