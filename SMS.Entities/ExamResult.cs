using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ExamResult:CommonProps
    {
        public int AcademicExamGroupId { get; set; }
        public AcademicExamGroup AcademicExamGroup { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public double TotalObtainMarks { get; set; }
        public double CGPA { get; set; }
        [StringLength(maximumLength:2)]
        public string FinalGrade { get; set; }
        public string GradeComments { get; set; }
        public int TotalFails { get; set; }
        public double AttendancePercentage { get; set; }
        public int AcademicClassId { get; set; }
        public int Rank { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public List<ExamResultDetail> ExamResultDetails { get; set; }

    }
}
