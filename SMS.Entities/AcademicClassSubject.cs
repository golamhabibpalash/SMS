using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AcademicClassSubject : CommonProps
    {
        public int AcademicSubjectId { get; set; }
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
        public bool Status { get; set; }
    }
}
