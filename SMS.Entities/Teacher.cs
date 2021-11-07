using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Teacher : CommonProps
    {
        [Display(Name="Teacher Name")]
        public string TeacherName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; }
    }
}
