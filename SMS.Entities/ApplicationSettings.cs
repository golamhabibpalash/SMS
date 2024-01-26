using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ApplicationSettings : CommonProps
    {
        [Required]
        public string SetupName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        [Required]
        public string ShortName { get; set; }
    }
}
