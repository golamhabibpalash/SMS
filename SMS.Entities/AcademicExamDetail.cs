using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExamDetail : CommonProps
    {
        public int AcademicExamId { get; set; }
        public AcademicExam AcademicExam { get; set; }
        public double ObtainMark { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
    }
}
