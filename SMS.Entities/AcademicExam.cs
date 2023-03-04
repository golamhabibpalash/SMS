using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExam : CommonProps
    {
        public string ExamName { get; set; }
        public int AcademicExamTypeId { get; set; }
        public int AcademicSessionId { get; set; }
        public int AcademicSubjectId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public AcademicExamType AcademicExamType { get; set; }
        public AcademicSession AcademicSession { get; set; }
        public AcademicSubject AcademicSubject { get; set; }

        public List<AcademicExamDetail> AcademicExamDetails { get; set; }
        public double TotalMarks { get; set; }

        public bool IsActive { get; set; }

    }
}
