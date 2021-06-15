using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class AttachDoc
    {
        public int Id { get; set; }
        public string DocumentsName { get; set; }
        public string Image { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
