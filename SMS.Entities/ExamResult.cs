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
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public double TotalObtainMarks { get; set; }
        public double CGPA { get; set; }
        public string FinalGrade { get; set; }
        public string GradeComments { get; set; }
        public int TotalFails { get; set; }
        public double AttendancePercentage { get; set; }
        public int MyProperty { get; set; }
        public List<AcademicExamDetail> AcademicExamDetails { get; set; }

    }
}
