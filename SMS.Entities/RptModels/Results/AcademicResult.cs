using System.Collections.Generic;

namespace SMS.Entities.RptModels.Results
{
    public class AcademicResult
    {
        public string ClassRoll { get; set; }
        public string StudentName { get; set; }
        List<SubjectResult> SubjectResults { get; set; }
        public double TotalMarks { get; set; }
        public double CGPA { get; set; }
        public string Grade { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
        public int PreviousMonthRank { get; set; }
    }
}