using SMS.Entities;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace SMS.App.ViewModels.ExamVM
{
    public class ExamDetailsVM
    {
        public string ExamName { get; set; }
        public int AcademicClassId { get; set; }
        public int AcademicSessionId { get; set; }
        public int AcademicSectionId { get; set; }
        public int TeacherId { get; set; }
        public string Teacher { get; set; }
        public string ExamMonth { get; set; }
        public int AcademicSubjectId { get; set; }
        public string AcademicSubjectName { get; set; }
        public double TotalMarks { get; set; }
        public double TotalTime { get; set; }
        public List<AcademicExamDetail> AcademicExamDetails { get; set; }
    }
}
