using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class QuestionFormat : CommonProps
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required] 
        public string QFormat { get; set; } //eg:1:4/1:3

        [Required] 
        public int NumberOfQuestion { get; set; } //eg:3/4/2
    }
}
