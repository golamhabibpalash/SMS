using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.Results
{
    public class SubjectWiseMarkSheetVM
    {
        public double ObtainMark { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public int ClassRoll { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
        public int AcademicExamId { get; set; }
        public string LetterGrade { get; set; }
        public decimal GradePoint { get; set; }

    }
}
