using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class QuestionDetails :CommonProps
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int QuestionTypeId { get; set; }
        public QuestionType QuestionType { get; set; }
        public string QuestionText { get; set; }
    }
}
