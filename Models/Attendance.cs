using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString ="{0:hh:mm:ss}")]
        public DateTime InTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        public DateTime OutTime { get; set; }

        [Display(Name = "Class")]
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }

        [Display(Name ="Session")]
        public int AcademicSessionId { get; set; }
        public AcademicSession AcademicSession { get; set; }
    }
}
