using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels.Results
{
    public class StudentWiseMarkSheetVM
    {
        public string Student_Name { get; set; }
        public int ClassRoll { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ReligionName { get; set; }
        public string Type { get; set; }
        public string SubjectName { get; set; }
        public int TotalMarks { get; set; }
        public double ObtainMark { get; set; }
        public double MaxNumber { get; set; }
        public string Grade { get; set; }
        public decimal Point { get; set; }
        public decimal FinalGPA { get; set; }
        public int ExamGroupId { get; set; }
        public int ClassId { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime DOB { get; set; }
        public string FinalGrade { get; set; }
        public int TotalFail { get; set; }
        public int Ranking { get; set; }
    }
}
