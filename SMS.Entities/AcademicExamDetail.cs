using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExamDetail : CommonProps
    {
        public int AcademicExamId { get; set; }
        public AcademicExam AcademicExam { get; set; }
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public double ObtainMark { get; set; }
        public double FullMark { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
