using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class StudentHub
    {
        public int Id { get; set; }
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }

        public int AcademicSectionId { get; set; }
        public AcademicSection AcademicSection { get; set; }

    }
}
