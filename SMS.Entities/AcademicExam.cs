using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicExam : CommonProps
    {
        public int AcademicExamGroupId { get; set; }
        public AcademicExamGroup AcademicExamGroup { get; set; }
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public int AcademicSectionId { get; set; }
        public AcademicSection AcademicSection { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public int TotalMarks { get; set; }
        public bool Status { get; set; }
    }
}
