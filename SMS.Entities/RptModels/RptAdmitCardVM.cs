using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities.RptModels
{
    public class RptAdmitCardVM
    {
        public int Id { get; set; }
        public int ClassRoll { get; set; }
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SectionName { get; set; }
        public string ClassName { get; set; }
        public int AcademicSectionId { get; set; }
        public int AcademicSubjectId { get; set; }
        public int MonthId { get; set; }
        public string ExamName { get; set; }
        public int SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int AcademicClassId { get; set; }
    }
}
