using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{ 
    public class Attendance
    {
        public int Id { get; set; }

        public DateTime AttendanceDate { get; set; }

        [Display(Name ="Attendance")]
        public bool IsPresent { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString ="{0:hh:mm:ss}")]
        public DateTime InTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}")]
        public DateTime OutTime { get; set; }

        [Display(Name ="User")]
        public int UserId { get; set; }
        public char UserType { get; set; }
    }
}
