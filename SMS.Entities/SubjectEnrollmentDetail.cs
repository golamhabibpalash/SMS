using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class SubjectEnrollmentDetail:CommonProps
    {
        public int SubjectEnrollmentId { get; set; }
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public int AcademicSubjectTypeId { get; set; }
        public AcademicSubjectType AcademicSubjectType { get; set; }
        public SubjectEnrollment SubjectEnrollment { get; set; }
        public bool IsOptional { get; set; }
    }
}
