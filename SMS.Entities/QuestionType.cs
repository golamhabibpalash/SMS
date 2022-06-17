using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class QuestionType : CommonProps
    {
        public string TypeName { get; set; } = string.Empty;
        public double MarkValue { get; set; }
    }
}
